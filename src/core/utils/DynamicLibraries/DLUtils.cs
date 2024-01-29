namespace SM64DSe.core.utils.DynamicLibraries
{
    public static class DLUtils
    {
        public static bool HasDynamicLibrarySupport()
        {
            bool autorw = Program.m_ROM.CanRW();
            if (!autorw) Program.m_ROM.BeginRW();
            
            if (Program.m_ROM.Read32(0x6590) != 0) //the patch makes this not 0
            {
                if (!autorw) Program.m_ROM.EndRW();
                return true;
            }
            if (!autorw) Program.m_ROM.EndRW();

            return false;
        }

        public static void ApplyDynamicLibraryPatch()
        {
            bool autorw = Program.m_ROM.CanRW();
            
            if (!autorw) Program.m_ROM.BeginRW();
            NitroOverlay ov2 = new NitroOverlay(Program.m_ROM, 2);

            //Move the ACTOR_SPAWN_TABLE so it can expand
            Program.m_ROM.WriteBlock(0x6590, Program.m_ROM.ReadBlock(0x90864, 0x61c));
            Program.m_ROM.WriteBlock(0x90864, new byte[0x61c]);

            //Adjust pointers
            Program.m_ROM.Write32(0x1a198, 0x02006590);

            //Move the OBJ_TO_ACTOR_TABLE so it can expand
            Program.m_ROM.WriteBlock(0x4b00, ov2.ReadBlock(0x0210cbf4 - ov2.GetRAMAddr(), 0x28c));
            ov2.WriteBlock(0x0210cbf4 - ov2.GetRAMAddr(), new byte[0x28c]);

            //Adjust pointers
            ov2.Write32(0x020fe890 - ov2.GetRAMAddr(), 0x02004b00);
            ov2.Write32(0x020fe958 - ov2.GetRAMAddr(), 0x02004b00);
            ov2.Write32(0x020fea44 - ov2.GetRAMAddr(), 0x02004b00);

            //Add the dynamic library loading and cleanup code
            Program.m_ROM.WriteBlock(0x90864, Properties.Resources.dynamic_library_loader);

            //Add the hooks (by replacing LoadObjBankOverlays())
            Program.m_ROM.WriteBlock(0x2df70, Properties.Resources.static_overlay_loader);

            if (!autorw) Program.m_ROM.EndRW();
            ov2.SaveChanges();
        }
    }
}