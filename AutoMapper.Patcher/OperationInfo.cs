using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Jitex.IL;
using MethodBody = Jitex.Builder.MethodBody;

namespace AutoMapper.Patcher
{
    internal class OperationInfo
    {
        public Operation Operation { get; }

        public bool IsVariable { get; }
        public bool IsLocalVariable { get; }
        public bool IsStaticVariable => ((FieldInfo)MemberInfo).IsStatic;

        public MemberInfo MemberInfo { get; }

        public OpCode SetOperator { get; }
        public OpCode LoadOperator { get; }

        public Type Type { get; }

        public OperationInfo(Operation operation, MethodBody body)
        {
            Operation = operation;
            MemberInfo = operation.Instance;

            IsVariable = operation.OpCode.Name.StartsWith("ldloc")
                         || operation.OpCode.Name.StartsWith("stloc")
                         || operation.OpCode.Name.StartsWith("ldsfld")
                         || operation.OpCode.Name.StartsWith("stsfld")
                         || operation.OpCode.Name.StartsWith("ldfld")
                         || operation.OpCode.Name.StartsWith("stfld");

            if (IsVariable)
            {
                bool isLoad = operation.OpCode.Name.StartsWith("ldloc");
                IsLocalVariable = isLoad || operation.OpCode.Name.StartsWith("stloc");

                if (IsLocalVariable)
                {
                    int localVarIndex;

                    int diff = OpCodes.Stloc_0.Value - OpCodes.Ldloc_0.Value;

                    if (isLoad)
                    {
                        if (operation.OpCode.Value >= OpCodes.Ldloc_0.Value && operation.OpCode.Value <= OpCodes.Ldloc_3.Value)
                        {
                            localVarIndex = Convert.ToInt32(operation.OpCode.Name[^1].ToString());
                        }
                        else
                        {
                            localVarIndex = operation.Instance;
                        }

                        LoadOperator = Operation.Translate((byte)(operation.OpCode.Value + diff));
                    }
                    else
                    {
                        if (operation.OpCode.Value >= OpCodes.Stloc_0.Value && operation.OpCode.Value <= OpCodes.Stloc_3.Value)
                        {
                            localVarIndex = Convert.ToInt32(operation.OpCode.Name[^1].ToString());
                        }
                        else
                        {
                            localVarIndex = operation.Instance;
                        }

                        LoadOperator = Operation.Translate((byte)(operation.OpCode.Value - diff));
                    }

                    Type = body.LocalVariables[localVarIndex].Type;
                }
                else if (MemberInfo is FieldInfo fieldInfo)
                {
                    Type = fieldInfo.FieldType;
                }
            }
            else if (MemberInfo is MethodInfo methodCall)
            {
                Type = methodCall.ReturnType;
            }
        }
    }
}
