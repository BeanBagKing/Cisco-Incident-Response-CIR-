@echo off
xsd /classes /out:. /language:CS /order /namespace:Recurity.CIR.Engine.Report report-common.xsd
move report-common.cs XmlReportPart.cs