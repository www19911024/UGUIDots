using System;
using System.Runtime.CompilerServices;
using UGUIDots.Transforms;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore;

namespace UGUIDots {

    // TODO: Rename this to the mesh rebuild tag
    /// <summary>
    /// Marks a mesh to be rebuilt.
    /// </summary>
    [Obsolete("This was originally a hack that should not be used...")]
    public struct BuildTextTag : IComponentData { }

    /// <summary>
    /// Stores a buffer of character values 
    /// </summary>
    // TODO: Represent chars as ushort instead?
    public struct CharElement : IBufferElementData {
        public char Value;

        public static implicit operator CharElement(char value) => new CharElement { Value = value };
        public static implicit operator char(CharElement value) => value.Value;
    }

    /// <summary>
    /// Stores glyph metric information to help generate the vertices required for the mesh.
    /// </summary>
    public struct GlyphElement : IBufferElementData {
        public ushort Unicode;

#if UNITY_EDITOR
        public char Char;
#endif

        public float Advance;
        public float2 Bearings;
        public float2 Size;

        /// <summary>
        /// Should be considered read only...use the extension functions to grab the UV coords
        /// </summary>
        public float2x4 UV;
        public FontStyle Style;
    }
    
    /// <summary>
    /// Stores the unique identifier for the text component's font.
    /// </summary>
    public struct TextFontID : IComponentData, IEquatable<TextFontID> {
        public int Value;

        public bool Equals(TextFontID other) {
            return other.Value == Value;
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
    }

    /// <summary>
    /// Stores stylizations of the text component.
    /// </summary>
    public struct TextOptions : IComponentData {
        public ushort Size;
        public FontStyle Style;
        public AnchoredState Alignment;
    }

    /// <summary>
    /// Stores the Font's unique identifier - this should typically be used on the "font" being converted.
    /// </summary>
    public struct FontID : IComponentData, IEquatable<FontID> {
        public int Value;

        public bool Equals(FontID other) {
            return other.Value == Value;
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
    }

    public static class GlyphExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 UVBottomLeft(this in GlyphElement element) {
            return element.UV.c0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static half2 UVBottomLeftAsHalf2(this in GlyphElement element) {
            return new half2(element.UVBottomLeft());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 UVBottomRight(this in GlyphElement element) {
            return element.UV.c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static half2 UVBottomRightAsHalf2(this in GlyphElement element) {
            return new half2(element.UVBottomRight());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 UVTopLeft(this in GlyphElement element) {
            return element.UV.c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static half2 UVTopLeftAsHalf2(this in GlyphElement element) {
            return new half2(element.UVTopLeft());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 UVTopRight(this in GlyphElement element) {
            return element.UV.c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static half2 UVTopRightAsHalf2(this in GlyphElement element) {
            return new half2(element.UVTopRight());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetGlyph(this in DynamicBuffer<GlyphElement> glyphs, in char c, in FontStyle style, out GlyphElement glyph) {
            var glyphArray = glyphs.AsNativeArray();
            return TryGetGlyph(in glyphArray, in c, in style, out glyph);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetGlyph(this in NativeArray<GlyphElement> glyphs, in char c, in FontStyle style, out GlyphElement glyph) {
            for (int i = 0; i < glyphs.Length; i++) {
                var current = glyphs[i];

                if (current.Unicode == (ushort)c && current.Style == style) {
                    glyph = current;
                    return true;
                }
            }

            glyph = default;
            return false;
        }
    }
    
    /// <summary>
    /// A copy of the FaceInfo struct generated by UnityEngine's TextCore.LowLevel;
    /// </summary>
    public struct FontFaceInfo : IComponentData {

        public int DefaultFontSize;
        public float AscentLine;
        public float BaseLine;
        public float CapLine;
        public float DescentLine;
        public FixedString32 FamilyName;
        public float LineHeight;
        public float MeanLine;
        public float PointSize;
        public float Scale;
        public float StrikeThroughOffset;
        public float StrikeThroughThickness;
        public float SubscriptSize;
        public float SubscriptOffset;
        public float SuperscriptSize;
        public float SuperscriptOffset;
        public float TabWidth;
        public float UnderlineOffset;
        public float UnderlineThickness;

        public static implicit operator FontFaceInfo(in FaceInfo info) {
            return new FontFaceInfo {
                AscentLine             = info.ascentLine,
                BaseLine               = info.baseline,
                CapLine                = info.capLine,
                DescentLine            = info.descentLine,
                FamilyName             = info.familyName,
                MeanLine               = info.meanLine,
                PointSize              = info.pointSize,
                Scale                  = info.scale,
                StrikeThroughThickness = info.strikethroughThickness,
                StrikeThroughOffset    = info.strikethroughThickness,
                SubscriptSize          = info.subscriptSize,
                SubscriptOffset        = info.subscriptOffset,
                SuperscriptSize        = info.superscriptSize,
                SuperscriptOffset      = info.superscriptOffset,
                TabWidth               = info.tabWidth,
                UnderlineOffset        = info.underlineOffset
            };
        }
    }
}
