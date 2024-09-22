﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.312
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 
namespace Recurity.CIR.Engine.PluginResults.Xml {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class ProcessRecordResult {
        
        private string processNameField;
        
        private ulong pidField;
        
        private ulong stackAddressField;
        
        private ulong stackAddressOldField;
        
        private ulong stackSizeField;
        
        private ulong stackLowLimitField;
        
        private ulong cpuTicksField;
        
        private float cpuUsage5secField;
        
        private float cpuUsage1minField;
        
        private float cpuUsage5minField;
        
        private ulong cpuInvokeField;
        
        private ulong memMallocField;
        
        private ulong memFreeField;
        
        private ulong memPoolAllocField;
        
        private ulong memPoolFreeField;
        
        private ulong callerField;
        
        private ulong calleeField;
        
        private bool isProfiledField;
        
        private bool isAnalyzedField;
        
        private bool isBlockedAtCrashField;
        
        private bool isCrashedField;
        
        private bool isKilledField;
        
        private bool isCorruptField;
        
        private bool isPreferringNewField;
        
        private bool isOnOldQueueField;
        
        private bool isWakeupPostedField;
        
        private bool isProfiledProcessField;
        
        private bool isProcessArgValidField;
        
        private bool isInitProcessField;
        
        private ulong processRecordAddressField;
        
        private bool fromProcessArrayField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string processName {
            get {
                return this.processNameField;
            }
            set {
                this.processNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public ulong pid {
            get {
                return this.pidField;
            }
            set {
                this.pidField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public ulong stackAddress {
            get {
                return this.stackAddressField;
            }
            set {
                this.stackAddressField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public ulong stackAddressOld {
            get {
                return this.stackAddressOldField;
            }
            set {
                this.stackAddressOldField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public ulong stackSize {
            get {
                return this.stackSizeField;
            }
            set {
                this.stackSizeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public ulong stackLowLimit {
            get {
                return this.stackLowLimitField;
            }
            set {
                this.stackLowLimitField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public ulong cpuTicks {
            get {
                return this.cpuTicksField;
            }
            set {
                this.cpuTicksField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public float cpuUsage5sec {
            get {
                return this.cpuUsage5secField;
            }
            set {
                this.cpuUsage5secField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public float cpuUsage1min {
            get {
                return this.cpuUsage1minField;
            }
            set {
                this.cpuUsage1minField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public float cpuUsage5min {
            get {
                return this.cpuUsage5minField;
            }
            set {
                this.cpuUsage5minField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
        public ulong cpuInvoke {
            get {
                return this.cpuInvokeField;
            }
            set {
                this.cpuInvokeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public ulong memMalloc {
            get {
                return this.memMallocField;
            }
            set {
                this.memMallocField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public ulong memFree {
            get {
                return this.memFreeField;
            }
            set {
                this.memFreeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=13)]
        public ulong memPoolAlloc {
            get {
                return this.memPoolAllocField;
            }
            set {
                this.memPoolAllocField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=14)]
        public ulong memPoolFree {
            get {
                return this.memPoolFreeField;
            }
            set {
                this.memPoolFreeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=15)]
        public ulong caller {
            get {
                return this.callerField;
            }
            set {
                this.callerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=16)]
        public ulong callee {
            get {
                return this.calleeField;
            }
            set {
                this.calleeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=17)]
        public bool isProfiled {
            get {
                return this.isProfiledField;
            }
            set {
                this.isProfiledField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=18)]
        public bool isAnalyzed {
            get {
                return this.isAnalyzedField;
            }
            set {
                this.isAnalyzedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=19)]
        public bool isBlockedAtCrash {
            get {
                return this.isBlockedAtCrashField;
            }
            set {
                this.isBlockedAtCrashField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=20)]
        public bool isCrashed {
            get {
                return this.isCrashedField;
            }
            set {
                this.isCrashedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=21)]
        public bool isKilled {
            get {
                return this.isKilledField;
            }
            set {
                this.isKilledField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=22)]
        public bool isCorrupt {
            get {
                return this.isCorruptField;
            }
            set {
                this.isCorruptField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=23)]
        public bool isPreferringNew {
            get {
                return this.isPreferringNewField;
            }
            set {
                this.isPreferringNewField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=24)]
        public bool isOnOldQueue {
            get {
                return this.isOnOldQueueField;
            }
            set {
                this.isOnOldQueueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=25)]
        public bool isWakeupPosted {
            get {
                return this.isWakeupPostedField;
            }
            set {
                this.isWakeupPostedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=26)]
        public bool isProfiledProcess {
            get {
                return this.isProfiledProcessField;
            }
            set {
                this.isProfiledProcessField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=27)]
        public bool isProcessArgValid {
            get {
                return this.isProcessArgValidField;
            }
            set {
                this.isProcessArgValidField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=28)]
        public bool isInitProcess {
            get {
                return this.isInitProcessField;
            }
            set {
                this.isInitProcessField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=29)]
        public ulong processRecordAddress {
            get {
                return this.processRecordAddressField;
            }
            set {
                this.processRecordAddressField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=30)]
        public bool fromProcessArray {
            get {
                return this.fromProcessArrayField;
            }
            set {
                this.fromProcessArrayField = value;
            }
        }
    }
}
