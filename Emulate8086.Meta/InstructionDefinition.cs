using System.Reflection;

namespace Emulate8086.Meta;

public class InstructionDefinition
{
    public InstructionTag Tag { get; }
    public InstructionOpcodePattern[] OpcodePatterns { get; }

    public InstructionDefinition(InstructionTag tag, params InstructionOpcodePattern[] opcodePatterns)
    {
        Tag = tag;
        OpcodePatterns = opcodePatterns;
    }

    public bool IsOpcodeMatch(byte opcode)
    {
        return OpcodePatterns.Any(pattern => pattern.IsOpcodeMatch(opcode));
    }

    public bool IsMatch(byte opcode, byte modRm)
    {
        var pattern = OpcodePatterns.SingleOrDefault(pattern => pattern.IsOpcodeMatch(opcode));
        if (pattern is null || pattern.ModRmOpcode is null)
        {
            return false;
        }

        return pattern.IsMatch(opcode, modRm);
    }

    public bool IsMatch(byte opcode)
    {
        return OpcodePatterns.Any(pattern => pattern.IsOpcodeMatch(opcode) && pattern.ModRmOpcode is null);
    }

    public string Name => Tag.Name;

    public override string ToString()
    {
        return Name;
    }

    public static InstructionDefinition[] GetAll(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType.IsSubclassOf(typeof(InstructionDefinition)))
            .Select(field => field.GetValue(null))
            .Cast<InstructionDefinition>()
            .ToArray();
    }
}
