<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:cirfmt="urn:CirFormatter" >


  <xsl:template match="CIR">
    <html>
      <head>
        <title>Detailed CIR Report</title>
        <link rel="stylesheet" type="text/css" href="cir.css" />

      </head>
      <body>
        <center>
          <img src="CIRlogo.png" alt="CIRlogo" width="260" heigth="103"/>
          <h1>Detailed Report</h1>
          <h3>
            Core dump platform: <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.ReportSignature.ReportSignature']/ReportSignaturesResult/CoreSignature" />
          </h3>
          <h4>
            <a href="report-summary.html">Summarized Report</a>
          </h4>
          <br />
        </center>
        <xsl:call-template name="basicreport"/>
        <xsl:call-template name="ReportSignaturedetails"/>
        <xsl:call-template name="TextSegmentCompare"/>
        <xsl:call-template name="RoDataSegmentCompare"/>
        <xsl:call-template name="IOSSignatureElf"/>
        <xsl:call-template name="IOSSignatureCore"/>
        <xsl:call-template name="ElfFileMemorydetails"/>
        <xsl:call-template name="IOHeapBlocksdetails"/>
        <xsl:call-template name="HeapBlocksdetails"/>
        <xsl:call-template name="PacketHeaderStructuredetails"/>
        <xsl:call-template name="ProcessLists"/>
        <xsl:call-template name="CheckHeaps" />
        <xsl:call-template name="CheckHeapUse" />
        <xsl:call-template name="licenseinfo"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template name="basicreport">
    <a name="basicreport"></a>
    <h2> Basic Summary</h2>

    <table cellspacing="0">
      <tr>
        <th style="background:#CCFFCC">Test passed</th>
        <th style="background:#FFFF99">Test execption</th>
        <th style="background:#FF9999">Test failed</th>
      </tr>
    </table>
    <br/>
    <table cellspacing="0">
      <tr>
        <th>Plugin Name</th>
        <th>Plugin Output</th>
      </tr>
      <xsl:for-each select="Report">
        <xsl:sort select="attribute::State" order="descending" data-type="number" />
        <xsl:if test="attribute::IsPlugin = 'true'">
          <tr>
            <xsl:if test="attribute::State = '0'">
              <xsl:attribute name="bgcolor">#CCFFCC</xsl:attribute>
            </xsl:if>
            <xsl:if test="attribute::State = '1'">
              <xsl:attribute name="bgcolor">#FFFF99</xsl:attribute>
            </xsl:if>
            <xsl:if test="attribute::State = '2'">
              <xsl:attribute name="bgcolor">#FF9999</xsl:attribute>
            </xsl:if>
            <td>
              <xsl:choose>
                <xsl:when test="attribute::Details = '1'">
                  <a>
                    <xsl:attribute name="href">
                      #<xsl:value-of select="cirfmt:formatAnker(attribute::Author)" />
                    </xsl:attribute>
                    <xsl:value-of select="attribute::Author" />
                  </a>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="attribute::Author" />
                </xsl:otherwise>
              </xsl:choose>
            </td>
            <td>
              <xsl:value-of select="Summary" />
            </td>
          </tr>
        </xsl:if>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template name="ReportSignaturedetails">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.ReportSignature.ReportSignature')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2> Signature Details</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.ReportSignature.ReportSignature']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.ReportSignature.ReportSignature']/@Description" />
      </p>
    </xsl:if>

    <table cellspacing="0">
      <tr>
        <th>Image Signature</th>
        <th>Core Signature</th>
      </tr>
      <tr>
        <td>
          <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.ReportSignature.ReportSignature']/ReportSignaturesResult/ImageSignature" />
        </td>
        <td>
          <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.ReportSignature.ReportSignature']/ReportSignaturesResult/CoreSignature" />
        </td>
      </tr>
    </table>
  </xsl:template>

  <xsl:template name="ElfFileMemorydetails">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.ELF.ElfFileMemory')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>Elf File Memory Details</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.ELF.ElfFileMemory']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.ELF.ElfFileMemory']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>Address</th>
        <th>Name</th>
        <th>Size</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.ELF.ElfFileMemory']/SegmentResult">

        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>

          <td>
            <xsl:value-of select="cirfmt:ULongToHex(./address)" />
          </td>
          <td>
            <xsl:value-of select="name" />
          </td>
          <td>
            <xsl:value-of select="size" />
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="IOHeapBlocksdetails">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.HeapParserIO.HeapParseIO')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>IO Heap Block Details</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.HeapParserIO.HeapParseIO']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.HeapParserIO.HeapParseIO']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>Address</th>
        <th>state</th>
        <th>by</th>
        <th>for</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.HeapParserIO.HeapParseIO']/HeapBlockResult">

        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(./address)" />
          </td>
          <td>
            <xsl:value-of select="state" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(by)" />
          </td>
          <td>
            <xsl:value-of select="for" />
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="HeapBlocksdetails">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.HeapParser.HeapParse')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>Heap Block Details</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.HeapParser.HeapParse']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.HeapParser.HeapParse']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>Address</th>
        <th>state</th>
        <th>by</th>
        <th>for</th>
        <th>PID</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.HeapParser.HeapParse']/HeapBlockResult">

        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(./address)" />
          </td>
          <td>
            <xsl:value-of select="state" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(by)" />
          </td>
          <td>
            <xsl:value-of select="for" />
          </td>
          <td>
            <xsl:value-of select="pid"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="PacketHeaderStructuredetails">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.ParsePacketHeaders.PacketHeaders')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>PacketHeaderStructure details</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.ParsePacketHeaders.PacketHeaders']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.ParsePacketHeaders.PacketHeaders']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>Address</th>
        <th>next</th>
        <th>frame</th>
        <th>size</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.ParsePacketHeaders.PacketHeaders']/PacketHeaderResult">

        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(./address)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(./next)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(./frame)" />
          </td>
          <td>
            <xsl:value-of select="size" />
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>


  <xsl:template name="IOSSignatureElf">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.ElfIOSSignature.ElfIOSSig')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>IOS Signature from Elf binary details</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.ElfIOSSignature.ElfIOSSig']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.ElfIOSSignature.ElfIOSSig']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>Image</th>
        <th>Family</th>
        <th>Featureset</th>
        <th>Version</th>
        <th>Media</th>
        <th>Detected Platform</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.ElfIOSSignature.ElfIOSSig']/SignatureResult">

        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <td>
            <xsl:value-of select="image" />
          </td>
          <td>
            <xsl:value-of select="family" />
          </td>
          <td>
            <xsl:value-of select="featureset" />
          </td>
          <td>
            <xsl:value-of select="version" />
          </td>
          <td>
            <xsl:value-of select="media" />
          </td>
          <td>
            <xsl:value-of select="detectedplatform" />
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="IOSSignatureCore">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.CoreIOSSignature.CoreIOSSig')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>IOS Signature from Core details</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.CoreIOSSignature.CoreIOSSig']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.CoreIOSSignature.CoreIOSSig']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>Image</th>
        <th>Family</th>
        <th>Featureset</th>
        <th>Version</th>
        <th>Media</th>
        <th>Detected Platform</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.CoreIOSSignature.CoreIOSSig']/SignatureResult">
        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <td>
            <xsl:value-of select="image" />
          </td>
          <td>
            <xsl:value-of select="family" />
          </td>
          <td>
            <xsl:value-of select="featureset" />
          </td>
          <td>
            <xsl:value-of select="version" />
          </td>
          <td>
            <xsl:value-of select="media" />
          </td>
          <td>
            <xsl:value-of select="detectedplatform" />
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="ProcessLists">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.ProcessList.ProcessLister')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>Process List</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.ProcessList.ProcessLister']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.ProcessList.ProcessLister']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>Source</th>
        <th>Process Name</th>
        <th>PID</th>
        <th>Stack Address</th>
        <th>Old Stack Address</th>
        <th>Stack Size</th>
        <th>Stack low limit</th>
        <th>CPU - ticks</th>
        <th>CPU usage 5 sec</th>
        <th>CPU usage 1 min</th>
        <th>CPU usage 5 min</th>
        <th>CPU invoke</th>
        <th>Memory allocated</th>
        <th>Memory free</th>
        <th>Memory-pool allocated</th>
        <th>Memory-pool free</th>
        <th>Caller</th>
        <th>Callee</th>
        <th>Profiled</th>
        <th>Analyzed</th>
        <th>Blocked at crash</th>
        <th>Crashed</th>
        <th>Killed</th>
        <th>Corrupt</th>
        <th>Preferring new</th>
        <th>On old queue</th>
        <th>Wakeup posted</th>
        <th>Profiled process</th>
        <th>Process arguments valid</th>
        <th>Init process</th>
        <th>Process record address</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.ProcessList.ProcessLister']/ProcessRecordResult">
        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <td>
            <xsl:if test="fromProcessArray = 'false'">
              <b>disinterred</b>
            </xsl:if>
            <xsl:if test="fromProcessArray = 'true'">
              Process Array
            </xsl:if>
          </td>
          <td>
            <xsl:value-of select="processName" />
          </td>
          <td>
            <xsl:value-of select="pid" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(stackAddress)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(stackAddressOld)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(stackSize)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(stackLowLimit)" />
          </td>
          <td>
            <xsl:value-of select="cpuTicks" />
          </td>
          <td>
            <xsl:value-of select="cpuUsage5sec" />%
          </td>
          <td>
            <xsl:value-of select="cpuUsage1min" />%
          </td>
          <td>
            <xsl:value-of select="cpuUsage5min" />%
          </td>
          <td>
            <xsl:value-of select="cpuInvoke" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(memMalloc)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(memFree)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(memPoolAlloc)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(memPoolFree)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(caller)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(callee)" />
          </td>
          <td>
            <xsl:value-of select="isProfiled" />
          </td>
          <td>
            <xsl:value-of select="isAnalyzed" />
          </td>
          <td>
            <xsl:value-of select="isBlockedAtCrash" />
          </td>
          <td>
            <xsl:value-of select="isCrashed" />
          </td>
          <td>
            <xsl:value-of select="isKilled" />
          </td>
          <td>
            <xsl:value-of select="isCorrupt" />
          </td>
          <td>
            <xsl:value-of select="isPreferringNew" />
          </td>
          <td>
            <xsl:value-of select="isOnOldQueue" />
          </td>
          <td>
            <xsl:value-of select="isWakeupPosted" />
          </td>
          <td>
            <xsl:value-of select="isProfiledProcess" />
          </td>
          <td>
            <xsl:value-of select="isProcessArgValid" />
          </td>
          <td>
            <xsl:value-of select="isInitProcess" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(processRecordAddress)" />
          </td>

        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="CheckHeaps">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.CheckHeaps.CheckHeap32')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>Check Heaps Results</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.CheckHeaps.CheckHeap32']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.CheckHeaps.CheckHeap32']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>Block address</th>
        <th>Issue</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.CheckHeaps.CheckHeap32']/CheckHeapReportResult">
        <tr>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(blockAddress)"/>
          </td>
          <td>
            <pre>
              <xsl:value-of select="issue"/>
            </pre>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="CheckHeapUse">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.CheckHeapUse.CheckHeapUse')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>Heap Usage Report</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.CheckHeapUse.CheckHeapUse']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.CheckHeapUse.CheckHeapUse']/@Description" />
      </p>
    </xsl:if>
    <table cellspacing="0">
      <tr>
        <th>PID</th>
        <th>Process Name</th>
        <th>Number of blocks</th>
        <th>Sum of bytes allocated (excl. heap overhead)</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.CheckHeapUse.CheckHeapUse']/CheckHeapUseReportResult">
        <tr>
          <td>
            <xsl:value-of select="pid"/>
          </td>
          <td>
            <xsl:value-of select="processName"/>
          </td>
          <td>
            <xsl:value-of select="numberOfBlock"/>
          </td>
          <td>
            <xsl:value-of select="sumOfBytes"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  
  <xsl:template name="TextSegmentCompare">
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.TextSegmentCompare.TextSegCompare']/@Details = '1'">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.TextSegmentCompare.TextSegCompare')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>Text Segment Compare</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.TextSegmentCompare.TextSegCompare']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.TextSegmentCompare.TextSegCompare']/@Description" />
      </p>
    </xsl:if>


    <table cellspacing="0">
      <tr>
        <th>Virtual Address</th>
        <th>Offset in ELF</th>
        <th>Offset in Core</th>
        <th>Length of diff</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.TextSegmentCompare.TextSegCompare']/SegmentDiffResult">
        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(virtualAddress)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(offsetElf)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(offsetCore)" />
          </td>
          <td>
            <xsl:value-of select="diffLength" />
          </td>
        </tr>
      </xsl:for-each>
    </table>
    </xsl:if>
  </xsl:template>

  <xsl:template name="RoDataSegmentCompare">
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.RoDataSegmentCompare.RoDataSegCompare']/@Details = '1'">
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="cirfmt:formatAnker('Recurity.CIR.Plugins.RoDataSegmentCompare.RoDataSegCompare')"></xsl:value-of>
      </xsl:attribute>
    </a>
    <h2>RoData Segment Compare</h2>
    <xsl:if test="Report[@Author='Recurity.CIR.Plugins.RoDataSegmentCompare.RoDataSegCompare']/@Description">
      <p>
        <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.RoDataSegmentCompare.RoDataSegCompare']/@Description" />
      </p>
    </xsl:if>


    <table cellspacing="0">
      <tr>
        <th>Virtual Address</th>
        <th>Offset in ELF</th>
        <th>Offset in Core</th>
        <th>Length of diff</th>
      </tr>
      <xsl:for-each select="Report[@Author='Recurity.CIR.Plugins.RoDataSegmentCompare.RoDataSegCompare']/SegmentDiffResult">
        <tr>
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">
              <xsl:attribute name="bgcolor">#EEEEEE</xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(virtualAddress)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(offsetElf)" />
          </td>
          <td>
            <xsl:value-of select="cirfmt:ULongToHex(offsetCore)" />
          </td>
          <td>
            <xsl:value-of select="diffLength" />
          </td>
        </tr>
      </xsl:for-each>
    </table>
    </xsl:if>
  </xsl:template>

  <xsl:template name="licenseinfo">
    <p style="font-size:9">
      <i>
        Licensed for: <xsl:value-of select="Report[@Author='LicenseInformation']/Summary" /> - <xsl:value-of select="Report[@Author='LicenseInformation']/@Description" />
      </i>
    </p>
  </xsl:template>


</xsl:stylesheet>