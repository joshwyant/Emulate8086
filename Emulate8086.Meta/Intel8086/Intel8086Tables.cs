namespace Emulate8086.Meta.Intel8086;
using static Intel8086InstructionTagValue;
using static Intel8086Tables.TableFlags;

public class Intel8086Tables
{
    // 16x16 matrix 0h-Fh x 0h-Fh
    // References 8088 Instruction Set Matrix, p. B-16
    // 8088 Instruction Reference, Technical Reference;
    // IBM Personal Computer XT Hardware Refernce Library, 1983
    public static readonly Intel8086InstructionTagValue[,] InstructionMatrix =
    {
        { ADD, ADD, ADD, ADD, ADD, ADD, PUSH, POP, OR, None, OR, OR, OR, OR, PUSH, None },
        { ADC, ADC, ADC, ADC, ADC, ADC, PUSH, POP, SBB, SBB, SBB, SBB, SBB, SBB, PUSH, POP },
        { AND, AND, AND, AND, AND, AND, ES, DAA, SUB, SUB, SUB, SUB, SUB, SUB, CS, DAS },
        { XOR, XOR, XOR, XOR, XOR, XOR, SS, AAA, CMP, CMP, CMP, CMP, CMP, CMP, DS, AAS },
        { INC, INC, INC, INC, INC, INC, INC, INC, DEC, DEC, DEC, DEC, DEC, DEC, DEC, DEC },
        { PUSH, PUSH, PUSH, PUSH, PUSH, PUSH, PUSH, PUSH, POP, POP, POP, POP, POP, POP, POP, POP },
        { None, None, None, None, None, None, None, None, None, None, None, None, None, None, None, None },
        { JO, JNO, JB, JNB, JE, JNE, JBE, JNBE, JS, JNS, JP, JNP, JL, JNL, JLE, JNLE },
        { Immediate, Immediate, Immediate, Immediate, TEST, TEST, XCHG, XCHG, MOV, MOV, MOV, MOV, MOV, LEA, MOV, POP },
        { NOP, XCHG, XCHG, XCHG, XCHG, XCHG, XCHG, XCHG, CBW, CWD, CALL, WAIT, PUSHF, POPF, SAHF, LAHF },
        { MOV, MOV, MOV, MOV, MOVS, MOVS, CMPS, CMPS, TEST, TEST, STOS, STOS, LODS, LODS, SCAS, SCAS },
        { MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV, MOV },
        { None, None, RET, RET, LES, LDS, MOV, MOV, None, None, RET, RET, INT, INT, INTO, IRET },
        { Shift, Shift, Shift, Shift, AAM, AAD, None, XLAT, ESC, ESC, ESC, ESC, ESC, ESC, ESC, ESC },
        { LOOPNZ, LOOPZ, LOOP, JCXZ, IN, IN, OUT, OUT, CALL, JMP, JMP, JMP, IN, IN, OUT, OUT },
        { LOCK, None, REP, REP, HLT, CMC, Group1, Group1, CLC, STC, CLI, STI, CLD, STD, Group2, Group2 }
    };
    public static readonly Intel8086InstructionTagValue[] ImmediateGroupInstructions
        = { ADD, OR, ADC, SBB, AND, SUB, XOR, CMP };
    public static readonly Intel8086InstructionTagValue[] ShiftGroupInstructions
        = { ROL, ROR, RCL, RCR, SHL, SHR, None, SAR };
    public static readonly Intel8086InstructionTagValue[] Group1Instructions
        = { TEST, None, NOT, NEG, MUL, IMUL, DIV, IDIV };
    public static readonly Intel8086InstructionTagValue[] Group2Instructions
        = { INC, DEC, CALL, CALL, JMP, JMP, PUSH, None };
    public static readonly TableFlags[,] FlagsMatrix =
    {
        { b|f|rm, w|f|rm, b|t|rm, w|t|rm, b|ia, w|ia, _, _, b|f|rm, w|f|rm, b|t|rm, w|t|rm, b|i, w|i, _, _ },
        { b|f|rm, w|f|rm, b|t|rm, w|t|rm, b|i, w|i, _, _,   b|f|rm, w|f|rm, b|t|rm, w|t|rm, b|i, w|i, _, _ },
        { b|f|rm, w|f|rm, b|t|rm, w|t|rm, b|i, w|i, _, _, b|f|rm, w|f|rm, b|t|rm, w|t|rm, b|i, w|i, _, _ },
        { b|f|rm, w|f|rm, b|t|rm, w|t|rm, b|i, w|i, _, _, b|f|rm, w|f|rm, b|t|rm, w|t|rm, b|i, w|i, _, _ },
        { _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _ },
        { _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _ },
        { _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _ },
        { _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _ },
        { b|rm, w|rm, b|rm, @is|rm, b|rm, w|rm, b|rm, w|rm, b|f|rm, w|f|rm, b|t|rm, w|t|rm, sr|t|rm, _, sr|f|rm, rm},
        { _, _, _, _, _, _, _, _, _, _, l|d, _, _, _, _, _, },
        { m, m, m, m, b, w, b, w, b|i, w|i, b, w, b, w, b, w },
        { i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i },
        { _, _, i, _, _, _, b|i|rm, w|i|rm, _, _, l|i, l, _, _, _, _ },
        { b, w, b|v, w|v, _, _, _, _, _, _, _, _, _, _, _, _ },
        { _, _, _, _, b, w, b, w, d, d, l|d, si|d, v|b, v|w, v|b, v|w },
        { _, _, _, z, _, _, b|rm, w|rm, _, _, _, _, _, _, b|rm, w|rm },
    };
    public static readonly TableFlags[] ImmediateGroupFlags = { rm, rm, rm, rm, rm, rm, rm, rm };
    public static readonly TableFlags[] ShiftGroupFlags = { rm, rm, rm, rm, rm, rm, _, rm };
    public static readonly TableFlags[] Group1Flags = { rm, _, rm, rm, rm, rm, rm, rm };
    public static readonly TableFlags[] Group2Flags = { rm, rm, rm | @id, rm | l | @id, rm | @id, rm | l | @id, rm, _ };
    [Flags]
    public enum TableFlags : ushort
    {
        _ = 0,
        b = 1 << 0,
        d = 1 << 1,
        f = 1 << 2,
        i = 1 << 3,
        ia = 1 << 4,
        id = 1 << 5,
        @is = 1 << 6,
        l = 1 << 7,
        m = 1 << 8,
        rm = 1 << 9,
        si = 1 << 10,
        sr = 1 << 11,
        t = 1 << 12,
        v = 1 << 13,
        w = 1 << 14,
        z = 1 << 15,
    }
}