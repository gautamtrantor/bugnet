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
        <p>К следующему заданию был добавлен новый комментарий.</p>
        <table border="0">
            <tr>
                <td width="90px" valign="top">
                    <b>Заголовок:</b>
                </td>
                <td>
                    <xsl:value-of select="Issue/Title" disable-output-escaping="yes" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Проект:</b>
                </td>
                <td>
                    <xsl:value-of select="Issue/ProjectName" disable-output-escaping="yes" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Автор:</b>
                </td>
                <td>
                    <xsl:value-of select="IssueComment/CreatorDisplayName" disable-output-escaping="yes" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Дата:</b>
                </td>
                <td>
                    <xsl:value-of select="helpers:FormatShortDateAnd12HTime(IssueComment/DateCreated)" />
                </td>
            </tr>
            <tr>
                <td><b>Комментарий:</b> </td>
            </tr>
            <tr>
                <td colspan="2">
                    <xsl:value-of select="IssueComment/Comment" disable-output-escaping="yes" />
                </td>
            </tr>
        </table>
        <p>
            Для получения более подробной информации об этом задании пройдите по ссылке
            <a href="{HostSetting_DefaultUrl}Issues/IssueDetail.aspx?id={Issue/Id}" target="_blank">
                <xsl:value-of select="HostSetting_DefaultUrl" />Issues/IssueDetail.aspx?id=<xsl:value-of select="Issue/Id" />
            </a>
        </p>
        <p style="text-align:center;font-size:8pt;padding:5px;">
            Если Вы больше не хотите получать данные уведомления, то посетите страницу <a href="{HostSetting_DefaultUrl}Account/UserProfile.aspx" target="_blank">Вашего профиля</a> и измените настройки уведомлений.
        </p>
    </xsl:template>
</xsl:stylesheet>
