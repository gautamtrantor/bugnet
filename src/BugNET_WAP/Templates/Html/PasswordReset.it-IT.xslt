﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns="http://schemas.microsoft.com/intellisense/ie5"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:helpers="urn:xsl-helpers">
    <xsl:output omit-xml-declaration="yes" method="html" />
    <xsl:strip-space elements="*" />
    <xsl:template match="/root">
        <p>Questa notifica conferma che hai cambiato la password del tuo account di <xsl:value-of select="HostSetting_ApplicationTitle" />.</p>
        <p>Se non hai modificato di tua iniziativa la password, contatta <xsl:value-of select="HostSetting_ApplicationTitle" /> per assistenza.</p>
        <p>
            Grazie!
            <br/>
            <xsl:value-of select="HostSetting_ApplicationTitle" />
        </p>
    </xsl:template>
</xsl:stylesheet>
