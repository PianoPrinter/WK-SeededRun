using System.Collections.Generic;
using Mono.Cecil;

public static class Patcher
{
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

    public static void Patch(AssemblyDefinition assembly)
    {
        var gsClass = assembly.MainModule.GetType("SettingsManager/GameSettings");
        gsClass.Fields.Add(new FieldDefinition("g_seeded", FieldAttributes.Public, assembly.MainModule.TypeSystem.Boolean));
    }
}
