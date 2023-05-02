using HarmonyLib;
using MonoMod.Utils;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace BeyondAbyss.Helper
{
    public static class Transpiler
    {
        private static readonly Dictionary<int, Delegate> DelegateCache = new Dictionary<int, Delegate>();
        private static int delegateCounter;
        /// <summary>Returns an instruction to call the specified delegate</summary>
		/// <typeparam name="T">The delegate type to emit</typeparam>
		/// <param name="action">The delegate to emit</param>
		/// <returns>The instruction to call the specified action</returns>
		///
		public static CodeInstruction EmitDelegate<T>(T action) where T : Delegate
        {
            if (action.Method.IsStatic && action.Target == null) return new CodeInstruction(OpCodes.Call, action.Method);

            var paramTypes = action.Method.GetParameters().Select(x => x.ParameterType).ToArray();

            var dynamicMethod = new DynamicMethodDefinition(action.Method.Name,
                action.Method.ReturnType,
                paramTypes);

            var il = dynamicMethod.GetILGenerator();

            var targetType = action.Target.GetType();

            var preserveContext = action.Target != null && targetType.GetFields().Any(x => !x.IsStatic);

            if (preserveContext)
            {
                var currentDelegateCounter = delegateCounter++;

                DelegateCache[currentDelegateCounter] = action;

                var cacheField = AccessTools.Field(typeof(Transpilers), nameof(DelegateCache));

                var getMethod = AccessTools.Method(typeof(Dictionary<int, Delegate>), "get_Item");

                il.Emit(OpCodes.Ldsfld, cacheField);
                il.Emit(OpCodes.Ldc_I4, currentDelegateCounter);
                il.Emit(OpCodes.Callvirt, getMethod);
            }
            else
            {
                if (action.Target == null)
                    il.Emit(OpCodes.Ldnull);
                else
                    il.Emit(OpCodes.Newobj,
                        AccessTools.FirstConstructor(targetType, x => x.GetParameters().Length == 0 && !x.IsStatic));

                il.Emit(OpCodes.Ldftn, action.Method);
                il.Emit(OpCodes.Newobj, AccessTools.Constructor(typeof(T), new[] { typeof(object), typeof(IntPtr) }));
            }

            for (var i = 0; i < paramTypes.Length; i++)
                il.Emit(OpCodes.Ldarg_S, (short)i);

            il.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(T), "Invoke"));
            il.Emit(OpCodes.Ret);

            return new CodeInstruction(OpCodes.Call, dynamicMethod.Generate());
        }
    }
}
