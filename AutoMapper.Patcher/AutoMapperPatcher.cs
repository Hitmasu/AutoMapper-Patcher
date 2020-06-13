using Jitex.IL;
using Jitex.JIT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using MethodBody = Jitex.Builder.MethodBody;

namespace AutoMapper.Patcher
{
    public class AutoMapperPatcher
    {
        private static readonly MethodInfo MethodMap;

        static AutoMapperPatcher()
        {
            MethodMap = typeof(IMapper).GetMethods().First(m => m.Name == "Map");
        }

        /// <summary>
        /// Initialize Jitex
        /// </summary>
        public static void Initialize()
        {
            ManagedJit jit = ManagedJit.GetInstance();

            jit.AddCompileResolver(CompileResolve);
        }

        /// <summary>
        /// Method resolver.
        /// </summary>
        /// <param name="context">Method will be compiled.</param>
        private static void CompileResolve(CompileContext context)
        {
            if (context.Method.Name == "MapWithJitex")
            {
                MethodBody body = new MethodBody((MethodInfo)context.Method);

                //All operations on method
                IList<Operation> operations = body.Operations.ToList();

                foreach (Operation operation in operations.Where(c => c.OpCode != OpCodes.Nop))
                {
                    if (operation.Instance is MethodInfo methodInfo && methodInfo.IsGenericMethod && methodInfo.GetGenericMethodDefinition() == MethodMap)
                    {
                        //Generic type passed: IMapper.Map<Type>();
                        Type typeDestination = methodInfo.GetGenericArguments().First();

                        //Variable passed by argument: IMapper.Map<Type>(variable);
                        Operation sourceOperation = null;

                        //Start block from Map call.
                        Operation startMapper = null;

                        //It's necessary find start operation of Mapper.Map call
                        //A basic call will generate these operations:

                        //IL_0000: ldsfld       IMapper
                        //IL_0005: ldarg.0      // this
                        //IL_0006: ldfld        Variable passed by argument
                        //IL_000b: callvirt     IMapper.Map<T>(obj)
                        //IL_0010: ret

                        //We are currently in IL_000b, so we need go until IL_0000.
                        //A way to do this, is check the type of variable|field until find a IMapper.
                        //If variable|field is typeof IMapper, that's start of operation block call, if not, is the variable|field passed by argument. 
                        for (int i = operation.Index - 1; i >= 0; i--)
                        {
                            Operation previousOperation = operations[i];

                            //Operation starts (IL_0000)
                            if (previousOperation.Instance is IMapper)
                            {
                                startMapper = previousOperation;
                                break;
                            }

                            //Field passed by argument in our case.
                            //That can be a variable (ldloc), method (callvirt|call), ...
                            if (previousOperation.OpCode.Name.StartsWith("ldfld"))
                            {
                                sourceOperation = previousOperation;
                            }
                        }

                        //Store all properties to make bind.
                        PropertyInfo[] sourceProperties = sourceOperation.Instance.FieldType.GetProperties();
                        PropertyInfo[] destionationProperties = typeDestination.GetProperties();
                        
                        //To start bind, we need firstly instantiate our destiny variable.
                        ConstructorInfo defaultConstructorDest = typeDestination.GetConstructor(Type.EmptyTypes);

                        List<byte> ilToReplace = new List<byte>
                        {
                            (byte) OpCodes.Newobj.Value
                        };

                        //TypeDestination variable = new TypeDestionation();
                        byte[] defaultCtor = BitConverter.GetBytes(defaultConstructorDest.MetadataToken);
                        ilToReplace.AddRange(defaultCtor);
                        
                        foreach (PropertyInfo sourceProperty in sourceProperties)
                        {
                            //Bind only property with same name (default config to AutoMapper)
                            PropertyInfo destProperty = destionationProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);

                            if (destProperty != null)
                            {
                                //Generate a simple get and set:
                                //variable.Property1 = variablePasseedByArgument.Property1

                                //Load field (_person) passed by argument on IMapper.Map
                                ilToReplace.Add((byte)OpCodes.Dup.Value);
                                ilToReplace.Add((byte)OpCodes.Ldarg_0.Value);
                                
                                ilToReplace.Add((byte)OpCodes.Ldfld.Value);
                                byte[] fieldToken = BitConverter.GetBytes(sourceOperation.MetadataToken.Value);
                                ilToReplace.AddRange(fieldToken);

                                //Load getter and setter
                                MethodInfo getMethod = sourceProperty.GetGetMethod();
                                MethodInfo setMethod = destProperty.GetSetMethod();

                                byte[] getToken = BitConverter.GetBytes(getMethod.MetadataToken);
                                byte[] setToken = BitConverter.GetBytes(setMethod.MetadataToken);

                                //callvirt instance variablePasseedByArgument.Property1 (get)
                                ilToReplace.Add((byte)OpCodes.Callvirt.Value);
                                ilToReplace.AddRange(getToken);

                                //callvirt instance variable.Property1 (set)
                                ilToReplace.Add((byte)OpCodes.Callvirt.Value);
                                ilToReplace.AddRange(setToken);
                            }
                        }

                        //Replace operations from AutoMapper
                        
                        int endMapperIndex = startMapper.ILIndex;

                        //All operations BEFORE IMapper.Map
                        byte[] startIL = body.IL[..endMapperIndex];

                        //All operations AFTER IMapper.Map
                        byte[] endIL = body.IL[(operation.ILIndex + operation.Size)..];

                        //The new IL generated
                        byte[] newIl = new byte[startIL.Length + endIL.Length + ilToReplace.Count];

                        Array.Copy(startIL, newIl, startIL.Length);
                        ilToReplace.CopyTo(newIl, startIL.Length);
                        Array.Copy(endIL, 0, newIl, startIL.Length + ilToReplace.Count, endIL.Length);

                        body.IL = newIl;
                    }
                }

                //Resolve method with our custom IL.
                context.ResolveBody(body);
            }
        }
    }
}