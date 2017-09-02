﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Commons.UI
{
    class Theme
    {
        public Color Text;
        public Color Warning;
        public Color Primary;
        public Color Accent;
        public Color Background;
    }

    class Script
    {
        public int FontSize;
        public FontStyle FontStyle;
        public bool Caps;
        public double Contrast;
    }

    class Typography
    {
        public Font font;
        public Script subhead;
        public Script title;
        public Script headline;
        public Script caption;
        public Script button;
        public Script body1;
        public Script body2;
    }

    class StyleProvider
    {
        public Vector4 shadow = new Vector4(180, 180, 180, 180);
        public Dictionary<string, Typography> typos;
        public Dictionary<string, Theme> themes;

        public StyleProvider()
        {
            themes = new Dictionary<string, Theme>();
            themes.Add("default", defaultTheme);
            typos = new Dictionary<string, Typography>();
            typos.Add("default", defaultTypo);
        }

        public void Config(string name, Theme theme)
        {
            if (themes.ContainsKey(name))
                themes[name] = theme;
            else
                themes.Add(name, theme);
        }

        public void Config(string name, Typography typo)
        {
            typos.Add(name, typo);
        }

        public void Apply(GameObject go, bool includeInactive = false, int depth = 10, string theme = "default", string typo = "default")
        {
            if (depth <= 0)
                return;
            string name = go.name.ToLower();
            if (name.Contains("theme="))
            {
                string[] res = Regex.Split(name, "theme=");
                res = res[1].Split(' ');
                theme = res[0];
            }
            if (name.Contains("typo="))
            {
                typo = "default";
            }
            ApplyTheme(go, theme);
            ApplyTypo(go, typo);
            if (go.transform.childCount > 0)
                for (int i = 0; i < go.transform.childCount; i++)
                    Apply(
                        go.transform.GetChild(i).gameObject,
                        includeInactive, depth - 1,
                        theme, typo);
        }

        private void ApplyTypo(GameObject go, string typo = "default")
        {
            Typography t = typos[typo];
            // TODO
        }

        private void ApplyTheme(GameObject go, string theme = "default")
        {
            Theme t = themes[theme];
            string name = go.name.ToLower();
            if (go.GetComponent<Text>() != null)
            {
                Text cmp = go.GetComponent<Text>();
                if (name.Contains("intention"))
                    if (name.Contains("intention=warning"))
                        cmp.color = t.Warning;
                    else if (name.Contains("intention=primary"))
                        cmp.color = t.Primary;
                    else
                        cmp.color = t.Text;
                else
                    cmp.color = t.Text;
            }
            if (go.GetComponent<Button>() != null)
            {
                Image cmp = go.GetComponent<Image>();
                cmp.sprite = null;
                cmp.color = t.Background;
            }
        }

        // TODO LOAD JSON FILE
        static Theme defaultTheme = new Theme()
        {
            Text = ColorHelper.HEXToRGB("212121"),
            Warning = ColorHelper.HEXToRGB("d84315"),
            Primary = ColorHelper.HEXToRGB("106cc8"),
            Accent = ColorHelper.HEXToRGB("ff5252"),
            Background = ColorHelper.HEXToRGB("fafafa")
        };
        static Typography defaultTypo = new Typography()
        {
            //  font = Resources.Load<Font>("Fonts/Roboto-Regular"),
            title = new Script()
            {
                FontSize = 16,
                FontStyle = FontStyle.Normal,
                Caps = false,
                Contrast = 0.87,
            },
            subhead = new Script()
            {
                FontSize = 16,
                FontStyle = FontStyle.Normal,
                Caps = false,
                Contrast = 0.87,
            },
            body1 = new Script()
            {
                FontSize = 14,
                FontStyle = FontStyle.Normal,
                Caps = false,
                Contrast = 0.87,
            },
            body2 = new Script()
            {
                FontSize = 14,
                FontStyle = FontStyle.Bold,
                Caps = false,
                Contrast = 0.87,
            },
            button = new Script()
            {
                FontSize = 14,
                FontStyle = FontStyle.Bold,
                Caps = true,
                Contrast = 1,
            }
        };
    }

    public static class ColorHelper
    {
        public static Color HEXToRGB(string hex)
        {
            if (hex.Length != 6)
                throw new ArgumentException("Length must be equal to 6");
            return new Color(
                int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) / 255f,
                int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) / 255f,
                int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) / 255f);
        }
    }
}
