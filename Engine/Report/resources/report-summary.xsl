<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">


<xsl:template match="CIR">
 <html><head><title>Summarized CIR Report</title>
 <link rel="stylesheet" type="text/css" href="cir.css" />

 </head><body>
 <center>
 <img src="CIRlogo.png" alt="CIRlogo" width="260" heigth="103"/>
 <h1>Summarzied Report</h1>
 <h3>Core dump platform: <xsl:value-of select="Report[@Author='Recurity.CIR.Plugins.ReportSignature.ReportSignature']/ReportSignaturesResult/CoreSignature" /></h3>
 <h4><a href="report-detailed.html">Detailed Report</a></h4> 
 
 <br />
 </center>
   <xsl:call-template name="basicreport"/>
   <xsl:call-template name="licenseinfo"/>
   
</body></html>
</xsl:template>

  <xsl:template name="licenseinfo">
    <p style="font-size:9">
      <i>
        Licensed for: <xsl:value-of select="Report[@Author='LicenseInformation']/Summary" /> - <xsl:value-of select="Report[@Author='LicenseInformation']/@Description" />
      </i>
    </p>
  </xsl:template>

<xsl:template name="basicreport">
<a name="basicreport"></a>
  <h2> Basic Summary</h2>

<table cellspacing="0"><tr><th style="background:#CCFFCC">Test passed</th><th style="background:#FFFF99">Test execption</th><th style="background:#FF9999">Test failed</th></tr></table>
<br/>
  <table cellspacing="0"><tr><th>Plugin Name</th><th>Plugin Output</th></tr>
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

          <xsl:value-of select="attribute::Author" />
        </td>
        <td>
          <xsl:value-of select="Summary" />
        </td>
      </tr>
    </xsl:if>
  </xsl:for-each>
</table>

</xsl:template>
</xsl:stylesheet>