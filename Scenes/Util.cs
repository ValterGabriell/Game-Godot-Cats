using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Logger
{
    /// <summary>
    /// Loga uma mensagem com timestamp, cor, estilos e origem (classe/método) do log.
    /// </summary>
    /// <param name="msg">Mensagem a ser exibida.</param>
    /// <param name="color">Cor da mensagem (ex: "red", "#00FF00", "blue").</param>
    /// <param name="withTimestamp">Se deve incluir timestamp.</param>
    /// <param name="bold">Se deve aplicar negrito.</param>
    /// <param name="italic">Se deve aplicar itálico.</param>
    /// <param name="underline">Se deve aplicar sublinhado.</param>
    /// <param name="callerName">Método chamador (preenchido automaticamente).</param>
    /// <param name="callerFilePath">Arquivo chamador (preenchido automaticamente).</param>
    public static void LogMessage(
        string msg,
        string color = "white",
        bool withTimestamp = true,
        bool bold = false,
        bool italic = false,
        bool underline = false,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "")
    {
        string timestamp = withTimestamp ? $"[color=gray]{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}[/color] - " : "";

        // Extrai apenas o nome da classe do caminho do arquivo
        string className = System.IO.Path.GetFileNameWithoutExtension(callerFilePath);

        string origin = $"[color=yellow]{className}.{callerName}()[/color]: ";

        string formattedMsg = msg;
        if (bold) formattedMsg = $"[b]{formattedMsg}[/b]";
        if (italic) formattedMsg = $"[i]{formattedMsg}[/i]";
        if (underline) formattedMsg = $"[u]{formattedMsg}[/u]";
        formattedMsg = $"[color={color}]{formattedMsg}[/color]";

        GD.PrintRich($"{timestamp}{origin}{formattedMsg}");
    }
}