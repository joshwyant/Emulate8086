namespace Emulate8086;

public interface IDevice
{
    void Out(int port, ref int val);
    void In(int port, int val);
}