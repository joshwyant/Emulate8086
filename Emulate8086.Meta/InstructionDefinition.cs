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

    public static Builder NewBuilder(InstructionTag tag)
    {
        return new Builder(tag);
    }

    public static Builder NewBuilder(InstructionDefinition def)
    {
        return new Builder(def);
    }

    public class Builder
    {
        private InstructionTag tag;
        private List<InstructionOpcodePattern> patterns;
        internal Builder(InstructionTag tag)
        {
            this.tag = tag;
            patterns = [];
        }
        public Builder(InstructionDefinition other)
        {
            tag = other.Tag;
            patterns = [.. other.OpcodePatterns];
        }
        public Builder WithTag(InstructionTag tag)
        {
            this.tag = tag;
            return this;
        }
        public Builder WithPattern(InstructionOpcodePattern pattern)
        {
            patterns.Add(pattern);
            return this;
        }
        public Builder WithPatterns(params InstructionOpcodePattern[] patterns)
        {
            this.patterns.AddRange(patterns);
            return this;
        }
        public Builder WithoutPattern(InstructionOpcodePattern pattern)
        {
            if (!patterns.Remove(pattern))
            {
                throw new KeyNotFoundException();
            }
            return this;
        }
        public InstructionDefinition Build()
        {
            return new InstructionDefinition(tag, [.. patterns]);
        }
    }
}
