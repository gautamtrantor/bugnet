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
        <p>Het volgende punt is bijgewerkt door <xsl:value-of select="Issue/LastUpdateDisplayName" disable-output-escaping="yes" />:</p>
        <table border="0">
            <tr>
                <td width="90px" valign="top">
                    <b>Titel:</b>
                </td>
                <td>
                    <xsl:value-of select="Issue/Title" disable-output-escaping="yes" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Project:</b>
                </td>
                <td>
                    <xsl:value-of select="Issue/ProjectName" disable-output-escaping="yes" />
                </td>
            </tr>
            <tr>
                <td colspan="2"><b>Wijzigingen:</b></td>
            </tr>
            <tr>
                <td colspan="2">
                    <xsl:if test="count(IssueHistoryChanges/IssueHistory) &gt; 0">
                        <ul>
                            <xsl:for-each select="IssueHistoryChanges/IssueHistory">
                                <li>
                                    <xsl:value-of select="FieldChanged" /> gewijzigd van "<xsl:value-of select="OldValue" disable-output-escaping="yes"/>" naar "<xsl:value-of select="NewValue" disable-output-escaping="yes"/>"
                                </li>
                            </xsl:for-each>
                        </ul>
                    </xsl:if>
                </td>
            </tr>
        </table>
        <p>
            Meer informatie over dit punt kunt u vinden via de volgende link
            <a href="{HostSetting_DefaultUrl}Issues/IssueDetail.aspx?id={Issue/Id}" target="_blank">
                <xsl:value-of select="HostSetting_DefaultUrl" />Issues/IssueDetail.aspx?id=<xsl:value-of select="Issue/Id" />
            </a>
        </p>
        <p style="text-align:center;font-size:8pt;padding:5px;">
            Als u geen meldingen meer wenst te ontvangen, ga dan naar <a href="{HostSetting_DefaultUrl}UserProfile.aspx" target="_blank">uw profiel</a> en wijzig uw meldings opties.
        </p>
    </xsl:template>
</xsl:stylesheet>
