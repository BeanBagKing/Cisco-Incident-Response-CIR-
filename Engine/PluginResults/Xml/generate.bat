@echo off
xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:SignatureResult schema.xsd
move schema.cs SignaturePart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:SegmentResult schema.xsd
move schema.cs SegmentPart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:PacketHeaderResult schema.xsd
move schema.cs PacketHeaderPart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:HeapBlockResult schema.xsd
move schema.cs HeapBlockPart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:SegmentDiffResult schema.xsd
move schema.cs SegmentDiffPart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:ReportSignaturesResult schema.xsd
move schema.cs ReportSignaturesPart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:ProcessRecordResult schema.xsd
move schema.cs ProcessRecordPart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:CheckHeapReportResult schema.xsd
move schema.cs CheckHeapReportResultPart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:CheckHeapUseReportResult schema.xsd
move schema.cs CheckHeapUseReportResultPart.cs

xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.PluginResults.Xml /e:RunPluginsResult schema.xsd
move schema.cs RunPluginsResultPart.cs