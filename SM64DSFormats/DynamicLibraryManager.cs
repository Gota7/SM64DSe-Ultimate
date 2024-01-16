using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace SM64DSe.SM64DSFormats
{
    public class DynamicLibraryManager
    {
        private readonly NitroOverlay _overlay;
        private readonly uint _tableAddr;
        private readonly List<ushort> _currentLibrariesInternalFileIds;
        private bool _isDirty;

        public DynamicLibraryManager(NitroOverlay mOverlay)
        {
            _overlay = mOverlay;
            _tableAddr = _overlay.Read32(0x30);
            _currentLibrariesInternalFileIds = new List<ushort>();
            _isDirty = true;
        }

        public void Clear()
        {
            // Clear current libraries
            _currentLibrariesInternalFileIds.Clear();
        }
        
        public void LoadCurrent()
        {
            Clear();
            
            // Reload from tables
            uint count = _overlay.Read16(_tableAddr);
            for (uint i = 0; i < count; i++)
            {
                _currentLibrariesInternalFileIds.Add(_overlay.Read16(i * 2 + _tableAddr + 2));
            }

            _isDirty = false;
        }
        
        public string[] GetCurrentLibrariesFilenames()
        {
            // Inside the overlay is saved a table of internal file id
            string[] current = new string[this.CountCurrent()];
            for (var i = 0; i < _currentLibrariesInternalFileIds.Count; i++)
            {
                current[i] = Program.m_ROM.GetFileFromInternalID(_currentLibrariesInternalFileIds[i]).m_Name;
            }
            return current;
        }

        public ushort[] GetCurrentLibrariesFileIds()
        {
            return _currentLibrariesInternalFileIds.ToArray();
        }

        public void Add(ushort internalFileId)
        {
            // Prevent from adding the same file multiple time
            if (_currentLibrariesInternalFileIds.Contains(internalFileId))
                throw new Exception("Cannot add the same file multiple time");
            
            _currentLibrariesInternalFileIds.Add(internalFileId);
            _isDirty = true;
        }

        private ushort GetInternalFileIdFromName(string filename)
        {
            return Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(filename)].InternalID;
        }
        
        public void Add(string filename)
        {
            Add(GetInternalFileIdFromName(filename));
        }
        
        public void Remove(ushort internalFileId)
        {
            // Throwing an error if trying to remove something not present
            if(!_currentLibrariesInternalFileIds.Contains(internalFileId))
                throw new Exception("File requested to remove does not exist.");

            _currentLibrariesInternalFileIds.Remove(internalFileId);
            _isDirty = true;
        }

        public void Remove(string filename)
        {
            Remove(GetInternalFileIdFromName(filename));
        }

        public int CountCurrent()
        {
            return _currentLibrariesInternalFileIds.Count;
        }

        // When marked as dirty, it mean the _currentLibraries is not sync with the actual values saved
        public bool IsDirty()
        {
            return _isDirty;
        }
    }
}