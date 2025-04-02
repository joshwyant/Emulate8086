namespace Emulate8086;

public interface IDevice
{
    void Out(int port, int val);
    void In(int port, ref int val);
}