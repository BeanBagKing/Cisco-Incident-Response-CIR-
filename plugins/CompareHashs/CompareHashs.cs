// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Robert Tezli (robert@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;

namespace Recurity.CIR.Plugins.CompareHashs
{
    public class CompareHashs : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        List<String> elfFiles = new List<string>();

        private char _delimeter = ',';
        public char Delimiter
        {
            get
            {
                return _delimeter;
            }
            set
            {
                _delimeter = value;
            }
        }

        private IElfCompressedFile elf;

        private string _csvpath = @"c:\test\test\csv";
        public string Csvpath
        {
            get { return _csvpath; }
            set { _csvpath = value; }
        }

        public override string Name
        {
            get { return "Elf hash check"; }
        }
        public override string Description
        {
            get { return "Compares the hash of a IOS image with ciscos hashtables"; }
        }
        protected override void InitializeInternal()
        {
        }
        protected override uint doStep()
        {
            // assign success here!
            bool success;
            if (Compare(elf.Info.FullName, _csvpath) != null)
            {
                success = true;
            }
            else
            {
                success = false;
            }
            HashCompareResult result = new HashCompareResult(this.GetType().ToString(), success, Compare(elf.Info.FullName, _csvpath));
            done(result);
            return 100;
        }
        protected override void CancelInternal()
        {
        }

        #region IAnalysisPlugin Members
        public CiscoPlatforms[] Platforms
        {
            get
            {
                CiscoPlatforms[] ret = { CiscoPlatforms.C2600, CiscoPlatforms.C1700 };
                return ret;

            }
        }
        public Type[] ResultTypes
        {
            get
            {
                Type[] ret = { typeof(IHeapStructureMain) };
                return ret;
            }
        }
        public IPluginResult[] Results
        {
            get
            {
                return ArrayMaker.Make(Result);
            }
        }
        public Type[] Requirements
        {
            get
            {
                Type[] ret = { typeof(IElfCompressedFile), typeof(IPluginConfiguration) };
                return ret;
            }
        }
        public void FulFill(object[] requirements)
        {
            this.elf = requirements[0] as IElfCompressedFile;
            IPluginConfiguration config = requirements[1] as IPluginConfiguration;
            //targetFilePath = /*Path.Combine(config.RuntimeConfiguration.OutputFolder, */@"c:\test\result.csv";
            //this._csvpath = @"c:\test\test\csv\merged\temp.csv";
        }
        #endregion

        private string Compare(string elfpath, string csvpath)
        {
            string hash = ComputeMD5(elfpath);
            string[] csvs = Directory.GetFiles(csvpath);

            foreach (string csv in csvs)
            {
                try
                {
                    object[] content = Read(csv);
                    foreach (string[] sa in content)
                    {
                        for (int i = 0; i < sa.Length; i++)
                        {
                            if (sa[i].ToUpper() == hash)
                            {
                                return "Your image has a match with: " + sa[i + 1];
                            }
                        }
                        //foreach (string s in sa)
                        //{
                        //    if (s.ToUpper() == hash)
                        //    {
                        //        //Console.WriteLine(s.ToUpper() + "  " + hash);
                        //        //Console.ReadLine();
                        //    }
                        //}
                    }

                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedAccessException(ex.Message);
                }
                catch (IOException ex)
                {
                    throw new IOException(ex.Message);
                }
                catch (OutOfMemoryException ex)
                {
                    throw new OutOfMemoryException(ex.Message);
                }

            }
            return null;
        }

        private object[] Read(string path)
        {
            ArrayList arrLst1 = new ArrayList();
            ArrayList arrLst2 = new ArrayList();
            string srLine = "";
            try
            {
                StreamReader sr = new StreamReader(path);
                while (srLine != null)
                {
                    srLine = sr.ReadLine();
                    if (srLine != null)
                    {
                        arrLst1.Add(srLine);
                    }
                }
                sr.Close();
                foreach (object item in arrLst1)
                {
                    arrLst2.Add(item.ToString().Split(_delimeter));
                }


            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message);
            }
            return arrLst2.ToArray();
        }
        private string ComputeMD5(string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                MD5 hash = new MD5CryptoServiceProvider();
                byte[] checkSums = hash.ComputeHash(fs);
                string outPut = BitConverter.ToString(checkSums).Replace("-", "").ToUpper();
                fs.Close();
                return outPut;
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
        }
    }
}
