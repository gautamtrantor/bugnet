<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:helpers="urn:xsl-helpers"
  exclude-result-prefixes="msxsl helpers">

    <xsl:output omit-xml-declaration="yes" method="html" />
    <xsl:strip-space elements="*" />

    <xsl:template match="/root">
        <p>如下用户已注册:</p>
        <table border="0">
            <tr>
                <td width="90px">
                    <b>日期:</b>
                </td>
                <td>
                    <xsl:value-of select="helpers:FormatShortDateAnd12HTime(NotificationUser/CreationDate)" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>姓名:</b>
                </td>
                <td>
                    <xsl:value-of select="NotificationUser/DisplayName" disable-output-escaping="yes" />
                </td>
            </tr>
            <tr>
                <td><b>用户名:</b> </td>
                <td>
                    <xsl:value-of select="NotificationUser/UserName" />
                </td>
            </tr>
            <tr>
                <td><b>电子邮件:</b> </td>
                <td>
                    <xsl:value-of select="NotificationUser/Email" />
                </td>
            </tr>
        </table>
    </xsl:template>
</xsl:stylesheet>
