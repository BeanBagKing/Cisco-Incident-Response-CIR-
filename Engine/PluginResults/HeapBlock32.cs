// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.PluginResults
{
    public class HeapBlock32 : IHeapBlock32
    {
        protected UInt64 _address;
        protected UInt32 _dataOffset;

        protected UInt32 _Magic;            // 0
        protected UInt32 _PID;              // 4
        protected UInt32 _AllocCheck;       // 8
        protected UInt32 _AllocName;        // 12
        protected UInt32 _AllocPC;          // 16
        protected UInt32 _NextBlock;        // 20
        protected UInt32 _PrevBlock;        // 24
        protected UInt32 _Size;             // 28
        protected UInt32 _RefCount;         // 32
        protected UInt32 _FreePC;           // 36        

        protected UInt32 _RedZone;
        protected IHeapBlockFree32 _FreeBlock;
        protected bool _used;
        protected string _allocNameString;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(
                "[HeapBlock {0,8:X}]\n" +
                " Magic     : {1,8:X}\n" +
                " PID       : {2,8:X}\n" +
                " Check     : {3,8:X}\n" +
                " Alloc PC  : {4,8:X}\n" +
                " Alloc Name: {5,8:X} (\"{6}\")\n" +
                " Next Block: {7,8:X}\n" +
                " Prev Block: {8,8:X}\n" +
                " Size      : {9,8:X} ({13})\n" +
                " References: {10,8:X}\n" +
                " Free PC   : {11,8:X}\n" +
                " REDZONE   : {12,8:X}\n",

                _address,
                _Magic,
                _PID,
                _AllocCheck,
                _AllocPC,
                _AllocName,
                _allocNameString,
                _NextBlock,
                _PrevBlock,
                _Size,
                _RefCount,
                _FreePC,
                _RedZone,
                _used?"used":"free"
            );

            return sb.ToString();
        }

        public UInt64 Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public UInt32 DataOffset
        {
            get { return _dataOffset; }
            set { _dataOffset = value; }
        }

        public UInt32 Magic
        {
            get { return _Magic; }
            set { _Magic = value; }
        }         

        public UInt32 PID
        {
            get { return _PID; }
            set { _PID = value; }
        }

        public UInt32 AllocCheck
        {
            get { return _AllocCheck; }
            set { _AllocCheck = value; }
        }

        public UInt32 AllocName
        {
            get { return _AllocName; }
            set { _AllocName = value; }
        }

        public UInt32 AllocPC
        {
            get { return _AllocPC; }
            set { _AllocPC = value; }
        }

        public UInt32 NextBlock
        {
            get { return _NextBlock; }
            set { _NextBlock = value; }
        }

        public UInt32 PrevBlock
        {
            get { return _PrevBlock; }
            set { _PrevBlock = value; }
        }

        public UInt32 Size
        {
            get { return _Size; }
            set { 
                _Size = value;
                
                if ( ( _Size & 0x80000000 ) == 0x80000000 )
                {
                    _used = true;
                }
                else
                {
                    _used = false;
                }

                unchecked
                {
                    _Size = _Size << 1;
                }
            }
        }

        public UInt32 RefCount
        {
            get { return _RefCount; }
            set { _RefCount = value; }
        }

        public UInt32 FreePC
        {
            get { return _FreePC; }
            set { _FreePC = value; }
        }

        public UInt32 RedZone
        {
            get { return _RedZone; }
            set { _RedZone = value; }
        }

        public bool Used
        {
            get { return _used; }
        }

        public UInt32 SizeFull
        {
            get { return _Size + _dataOffset; }
        }

        public UInt32 HeaderSize
        {
            get { return _dataOffset; }
        }

        public string AllocNameString
        {
            get { return _allocNameString; }
            set { _allocNameString = value; }
        }

        public IHeapBlockFree32 FreeBlock
        {
            get { return _FreeBlock; }
            set { _FreeBlock = value; }
        }
    }
}
