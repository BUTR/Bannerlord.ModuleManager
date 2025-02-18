#region License
// MIT License
//
// Copyright (c) Bannerlord's Unofficial Tools & Resources
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#nullable enable
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning disable
#endif

namespace Bannerlord.ModuleManager
{
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        sealed record ModuleIssue
    {
        public ModuleInfoExtended Target { get; set; }
        public string SourceId { get; set; }
        public ModuleIssueType Type { get; set; }
        public string Reason { get; set; }
        public ApplicationVersionRange SourceVersion { get; set; }

        public ModuleIssue()
        {
            Target = new();
            SourceId = string.Empty;
            Type = ModuleIssueType.NONE;
            Reason = string.Empty;
            SourceVersion = ApplicationVersionRange.Empty;
        }
        public ModuleIssue(ModuleInfoExtended target, string sourceId, ModuleIssueType type)
        {
            Target = target;
            SourceId = sourceId;
            Type = type;
            Reason = string.Empty;
            SourceVersion = ApplicationVersionRange.Empty;
        }
        public ModuleIssue(ModuleInfoExtended target, string sourceId, ModuleIssueType type, string reason, ApplicationVersionRange sourceVersion) : this(target, sourceId, type)
        {
            Reason = reason;
            SourceVersion = sourceVersion;
        }
    }
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}