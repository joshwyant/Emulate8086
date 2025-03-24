using System.Reflection;

namespace Emulate8086.Meta;

public abstract class InstructionTag : IEquatable<InstructionTag>
{
    public abstract bool Equals(InstructionTag? other);

    public static InstructionTag[] GetAll(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType.IsSubclassOf(typeof(InstructionTag)))
            .Select(field => field.GetValue(null))
            .Cast<InstructionTag>()
            .Where(tag => tag.Name != "None")
            .ToArray();
    }

    public abstract string Name { get; }
}

public class InstructionTag<T> : InstructionTag
    where T : Enum
{
    public T Value { get; }

    public InstructionTag(T value)
    {
        Value = value;
    }

    public override bool Equals(InstructionTag? other)
    {
        if (other is null)
        {
            return false;
        }
        if (other is InstructionTag<T> otherTag)
        {
            return EqualityComparer<T>.Default.Equals(Value, otherTag.Value);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }

    public override string Name => Value.ToString();

    public override string ToString() => Name;
}

