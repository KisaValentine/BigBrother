using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

//Thanks https://github.com/Ottermandias/Glamourer/blob/b65658ef6337137c5d6d8aeff8620ee087f2d951/Glamourer/Gui/ImGuiRaii.cs
namespace BigBrother.Windows
{
    public sealed class ImGuiRaii : IDisposable
    {
        private int colorStack;
        private int fontStack;
        private int styleStack;
        private float indentation;

        private Stack<Action>? onDispose;

        public static ImGuiRaii NewGroup()
            => new ImGuiRaii().Group();

        public ImGuiRaii Group()
            => Begin(ImGui.BeginGroup, ImGui.EndGroup);

        public static ImGuiRaii NewTooltip()
            => new ImGuiRaii().Tooltip();

        public ImGuiRaii Tooltip()
            => Begin(ImGui.BeginTooltip, ImGui.EndTooltip);

        public ImGuiRaii PushColor(ImGuiCol which, uint color)
        {
            ImGui.PushStyleColor(which, color);
            ++colorStack;
            return this;
        }

        public ImGuiRaii PushColor(ImGuiCol which, Vector4 color)
        {
            ImGui.PushStyleColor(which, color);
            ++colorStack;
            return this;
        }

        public ImGuiRaii PopColors(int n = 1)
        {
            var actualN = Math.Min(n, colorStack);
            if (actualN > 0)
            {
                ImGui.PopStyleColor(actualN);
                colorStack -= actualN;
            }

            return this;
        }

        public ImGuiRaii PushStyle(ImGuiStyleVar style, Vector2 value)
        {
            ImGui.PushStyleVar(style, value);
            ++styleStack;
            return this;
        }

        public ImGuiRaii PushStyle(ImGuiStyleVar style, float value)
        {
            ImGui.PushStyleVar(style, value);
            ++styleStack;
            return this;
        }

        public ImGuiRaii PopStyles(int n = 1)
        {
            var actualN = Math.Min(n, styleStack);
            if (actualN > 0)
            {
                ImGui.PopStyleVar(actualN);
                styleStack -= actualN;
            }

            return this;
        }

        public ImGuiRaii PushFont(ImFontPtr font)
        {
            ImGui.PushFont(font);
            ++fontStack;
            return this;
        }

        public ImGuiRaii PopFonts(int n = 1)
        {
            var actualN = Math.Min(n, fontStack);

            while (actualN-- > 0)
            {
                ImGui.PopFont();
                --fontStack;
            }

            return this;
        }

        public ImGuiRaii Indent(float width)
        {
            if (width != 0)
            {
                ImGui.Indent(width);
                indentation += width;
            }

            return this;
        }

        public ImGuiRaii Unindent(float width)
            => Indent(-width);

        public bool Begin(Func<bool> begin, Action end)
        {
            if (begin())
            {
                onDispose ??= new Stack<Action>();
                onDispose.Push(end);
                return true;
            }

            return false;
        }

        public ImGuiRaii Begin(Action begin, Action end)
        {
            begin();
            onDispose ??= new Stack<Action>();
            onDispose.Push(end);
            return this;
        }

        public void End(int n = 1)
        {
            var actualN = Math.Min(n, onDispose?.Count ?? 0);
            while (actualN-- > 0)
                onDispose!.Pop()();
        }

        public void Dispose()
        {
            Unindent(indentation);
            PopColors(colorStack);
            PopStyles(styleStack);
            PopFonts(fontStack);
            if (onDispose != null)
            {
                End(onDispose.Count);
                onDispose = null;
            }
        }
    }
}
