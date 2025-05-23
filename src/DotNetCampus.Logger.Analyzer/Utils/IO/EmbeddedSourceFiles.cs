﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotNetCampus.Logger.Utils.IO;

/// <summary>
/// 从嵌入的资源中寻找源代码。
/// </summary>
internal static class EmbeddedSourceFiles
{
    /// <summary>
    /// 寻找 <paramref name="folderName"/> 文件夹下的源代码名称和内容。
    /// </summary>
    /// <param name="folderName">资源文件夹名称。请以“/”或“\”分隔文件夹。</param>
    /// <returns></returns>
    internal static IEnumerable<EmbeddedSourceFile> Enumerate(string? folderName)
    {
        // 资源字符串格式为："{Namespace}.{Folder}.{filename}.{Extension}"
        var desiredFolder = $"{typeof(GeneratorInfo).Namespace}{(folderName is null ? "" : "." + folderName)}";
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            var prefix = desiredFolder.Replace('/', '.').Replace('\\', '.') + ".";
            if (resourceName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                using var stream = assembly.GetManifestResourceStream(resourceName)!;
                using var reader = new StreamReader(stream);
                var contentText = reader.ReadToEnd();

                var fileName = resourceName.AsSpan().Slice(prefix.Length).ToString();
                var fileNameWithoutExtension = fileName.Replace(".g.cs", "").Replace(".cs", "");
                var fileExtension = fileName.Replace(fileNameWithoutExtension, "");
                var fileNameIndex = fileNameWithoutExtension.LastIndexOf('.');
                if (fileNameIndex < 0)
                {
                    // 文件直接在此文件夹中。
                    yield return new EmbeddedSourceFile(
                        fileName,
                        fileName,
                        fileNameWithoutExtension,
                        typeof(GeneratorInfo).Namespace!,
                        contentText);
                }
                else
                {
                    // 文件在子文件夹中。
                    var typeName = fileNameWithoutExtension.Substring(fileNameIndex + 1);
                    var relativeFolder = fileNameWithoutExtension.Substring(0, fileNameIndex);
                    var @namespace = $"{typeof(GeneratorInfo).Namespace}.{relativeFolder}";
                    yield return new EmbeddedSourceFile(
                        $"{typeName}{fileExtension}",
                        $"{relativeFolder.Replace(".", "/")}/{typeName}{fileExtension}",
                        typeName,
                        @namespace,
                        contentText);
                }
            }
        }
    }
}
