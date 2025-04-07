// using Emulate8086.Processor;

// namespace Emulate8086.BIOS;

// public class PCBIOS
// {
//     // Reference: IBM PC Technical Reference Appendix A: ROM BIOS Listings
//     CPU cpu;
//     public BIOS(CPU cpu)
//     {
//         this.cpu = cpu;
//     }
//     #region Equates
//     /// <summary>
//     /// 8225 Port A Address
//     /// </summary>
//     const int PORT_A = 0x60;
//     /// <summary>
//     /// 8225 Port B Address
//     /// </summary>
//     const int PORT_B = 0x61;
//     /// <summary>
//     /// 8225 Port C Address
//     /// </summary>
//     const int PORT_C = 0x62;
//     const int CMD_PORT = 0x63;
//     /// <summary>
//     /// 8259 Port
//     /// </summary>
//     const int INTA00 = 0x20;
//     /// <summary>
//     /// 8259 Port
//     /// </summary>
//     const int INTA01 = 0x21;
//     const int EOI = 0x20;
//     const int TIMER = 0x40;
//     /// <summary>
//     /// 8253 Timer Control Port Address
//     /// </summary>
//     const int TIM_CTL = 0x43;
//     /// <summary>
//     /// 8253 Timer/Counter 0 Port Address
//     /// </summary>
//     const int TIMER0 = 0x40;
//     /// <summary>
//     /// Timer 0 Interrupt Received Mask
//     /// </summary>
//     const int TMINT = 0x01;
//     /// <summary>
//     /// DMA Status Reg Port Address
//     /// </summary>
//     const int DMA08 = 0x08;
//     /// <summary>
//     /// DMA Channel 0 Address Reg Port Address
//     /// </summary>
//     const int DMA = 0x00;
//     const int MAX_PERIOD = 0x540;
//     const int MIN_PERIOD = 0x410;
//     /// <summary>
//     /// Keyboard Data In Port
//     /// </summary>
//     const int KBD_IN = 0x60;
//     /// <summary>
//     /// Keyboard Interrupt Mask
//     /// </summary>
//     const int KBDINT = 0x02;
//     /// <summary>
//     /// Keyboard Scan Code Port
//     /// </summary>
//     const int KB_DATA = 0x60;
//     /// <summary>
//     /// Control Bits for KB Sense Data
//     /// </summary>
//     const int KB_CTL = 0x61;
//     #endregion
//     #region 8088 Interrupt Locations
//     const int STG_LOC0 = 0;
//     const int NMI_PTR = 2 * 4;
//     const int INT5_PTR = 5 * 4;
//     const int INT_ADDR = 8 * 4;
//     const int INT_PTR = 8 * 4;
//     const int VIDEO_INT = 0x10 * 4;
//     /// <summary>
//     /// Pointer to video parms
//     /// </summary>
//     const int PARM_PTR = 0x1D * 4;
//     /// <summary>
//     /// Entry point for cassette basic
//     /// </summary>
//     const int BASIC_PTR = 0x18 * 4;
//     /// <summary>
//     /// Interrupt 1Eh
//     /// </summary>
//     const int DISK_POINTER = 0x1E * 4;
//     /// <summary>
//     /// Pointer to extension routine
//     /// </summary>
//     const int EXT_PTR = 0x1F * 4;
//     const int IO_ROM_INIT = 0x40 * 4;
//     /// <summary>
//     /// Optional ROM segment
//     /// </summary>
//     const int IO_ROM_SEG = IO_ROM_INIT + 2;
//     /// <summary>
//     /// Absolute location of data segment
//     /// </summary>
//     const int DATA_AREA = 0x400;
//     const int DATA_WORD = DATA_AREA;
//     const int BOOT_LOCN = 0x7C00;
//     #endregion
//     #region ROM BIOS Data Areas
//     const int RS232_BASE;
//     const int PRINTER_BASE;
//     const int EQUIP_FLAG;
//     const int MFG_TST;
//     const int MEMORY_SIZE;
//     const int IO_RAM_SIZE;
//     #endregion
//     #region Keyboard Data Areas
//     const int KB_FLAG;
//     const int INS_STATE;
//     const int CAPS_STATE;
//     const int NUM_STATE;
//     const int SCROLL_STATE;
//     const int ALT_SHIFT;
//     const int CTL_SHIFT;
//     const int LEFT_SHIFT;
//     const int RIGHT_SHIFT;
//     const int KB_FLAG_1;
//     const int INS_SHIFT;
//     const int CAPS_SHIFT;
//     const int NUM_SHIFT;
//     const int SCROLL_SHIFT;
//     const int HOLD_STATE;
//     const int ALT_INPUT;
//     const int BUFFER_HEAD;
//     const int BUFFER_TAIL;
//     const int KB_BUFFER;
//     const int KB_BUFFER_END;
//     const int NUM_KEY;
//     const int SCROLL_KEY;
//     const int ALT_KEY;
//     const int CTL_KEY;
//     const int CAPS_KEY;
//     const int LEFT_KEY;
//     const int RIGHT KEY;
//     const int INS_KEY;
//     const int DEL_KEY;
//     #endregion
//     #region Diskette Data Areas
//     const int SEEK_STATUS;
//     const int INT_FLAG;
//     const int MOTOR_STATUS;
//     const int MOTOR_WAIT;
//     const int DISKETTE_STATUS;
//     const int TIME_OUT;
//     const int BAD_SEEK;
//     const int BAD_NEC;
//     const int BAD_CRC;
//     const int DMA_BOUNDARY;
//     const int BAD_DMA;
//     const int RECORD_NOT_FND;
//     const int WRITE_PROTECT;
//     const int BAD_ADDR_MARK;
//     const int BAD_CMD;
//     const int NEC_STATUS;
//     #endregion
//     #region Video Display Data Area
//     #endregion
//     #region Timer Data Area
//     #endregion
//     #region System Data Area
//     #endregion
//     #region Fixed Disk Data Area
//     #endregion
//     #region Printer and RS232 Timeout Counters
//     #endregion
//     #region Extra Keyboard Data Area
//     #endregion
//     #region Extra Data Area
//     #endregion
//     #region Video Display Buffer
//     #endregion
//     #region ROM Resident Code
//     #endregion
//     #region Initial Reliability Tests -- Phase 1
//     #endregion
//     #region Data Definitions
//     #endregion
// }