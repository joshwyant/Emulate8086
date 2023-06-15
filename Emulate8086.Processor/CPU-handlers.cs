using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        bool is_seg_prefix = false;
        Register seg_prefix = Register.None;

        private static void HandleNone(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleImmediate(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleShift(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleGroup1(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleGroup2(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleESPrefix(CPU self)
        {
            self.is_seg_prefix = true;
            self.seg_prefix = Register.ES;
        }

        private static void HandleCSPrefix(CPU self)
        {
            self.is_seg_prefix = true;
            self.seg_prefix = Register.CS;
        }

        private static void HandleSSPrefix(CPU self)
        {
            self.is_seg_prefix = true;
            self.seg_prefix = Register.SS;
        }

        private static void HandleDSPrefix(CPU self)
        {
            self.is_seg_prefix = true;
            self.seg_prefix = Register.DS;
        }

        private static void HandleAAA(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleAAD(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleAAM(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleAAS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleADC(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleADD(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleAND(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCALL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCBW(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCLC(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCLD(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCLI(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCMC(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCMP(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCMPS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleCWD(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleDAA(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleDAS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleDEC(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleDIV(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleESC(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleHLT(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleIDIV(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleIMUL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleIN(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleINC(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleINT(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleINTO(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleIRET(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJA(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJAE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJB(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJBE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJCXZ(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJG(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJGE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJLE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJMP(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNA(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNAE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNB(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNBE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNG(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNGE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNLE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNO(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNP(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJNZ(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJO(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJP(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJPE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJPO(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleJZ(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLAHF(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLDS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLEA(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLES(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLOCK(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLODS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLOOP(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLOOPE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLOOPNE(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLOOPNZ(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleLOOPZ(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleMOVS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleMUL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleNEG(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleNOP(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleNOT(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleOR(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleOUT(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandlePOP(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandlePOPF(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandlePUSH(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandlePUSHF(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleRCL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleRCR(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleREP(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleRET(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleROL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleROR(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSAHF(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSAL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSAR(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSBB(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSCAS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSHL(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSHR(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSTC(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSTD(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSTI(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSTOS(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleSUB(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleTEST(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleWAIT(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleXCHG(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleXLAT(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleXOR(CPU self)
        {
            throw new NotImplementedException();
        }
    }
}
