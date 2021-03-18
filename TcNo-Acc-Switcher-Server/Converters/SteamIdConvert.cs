﻿using System;
using System.Globalization;
using System.Linq;

namespace TcNo_Acc_Switcher_Server.Converters
{
    internal class SteamIdConvert
    {
        // Usage:
        // - Console.WriteLine(new SteamIdConvert("STEAM_0:0:52161201").PrintAll());
        // - SteamIdConvert sid = new SteamIdConvert("STEAM_0:0:52161201");
        //   string Steam64 = sid.Id64;

        public string Id = "STEAM_0:", Id3 = "U:1:", Id32, Id64;

        private string _input;
        private byte _inputType;

        private static readonly char[] Sid3Strings = { 'U', 'I', 'M', 'G', 'A', 'P', 'C', 'g', 'T', 'L', 'C', 'a' };
        private const byte SteamId = 1, SteamId3 = 2, SteamId32 = 3, SteamId64 = 4;
        private const long ChangeVal = 76561197960265728;

        public SteamIdConvert(string anySteamId)
        {
            try
            {
                GetIdType(anySteamId);
                ConvertAll();
            }
            catch (SteamIdConvertException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void GetIdType(string sInput)
        {
            _input = sInput;
            if (_input[0] == 'S')
            {
                _inputType = 1; // SteamID
            }
            else if (Sid3Strings.Contains(_input[0]))
            {
                _inputType = 2; // SteamID3
            }
            else if (char.IsNumber(_input[0]))
            {
                _inputType = _input.Length switch
                {
                    < 17 => 3,
                    17 => 4,
                    _ => _inputType
                };
            }
            else
            {
                throw new SteamIdConvertException("Input SteamID was not recognised!");
            }
        }

        private static string GetOddity(string input)
        {
            return (int.Parse(input) % 2).ToString();
        }

        private static string FloorDivide(string sIn, int divIn)
        {
            return Math.Floor((double)(int.Parse(sIn) / divIn)).ToString(CultureInfo.InvariantCulture);
        }

        private string CalcSteamId()
        {
            if (_inputType == SteamId)
            {
                Id = _input;
            }
            else
            {
                var s = _inputType switch
                {
                    SteamId3 => _input.Substring(4),
                    SteamId32 => _input,
                    SteamId64 => CalcSteamId32(),
                    _ => ""
                };

                Id += GetOddity(s) + ":" + FloorDivide(s, 2);
            }

            return Id;
        }

        private string CalcSteamId3()
        {
            if (_inputType == SteamId3)
                Id3 = _input;
            else
                Id3 += CalcSteamId32();
            Id3 = $"[{Id3}]";
            return Id3;
        }

        private string CalcSteamId32()
        {
            if (_inputType == SteamId32)
            {
                Id32 = _input;
            }
            else
            {
                Id32 = _inputType switch
                {
                    SteamId => (int.Parse(_input.Substring(10)) * 2 + int.Parse($"{_input[8]}")).ToString(),
                    SteamId3 => _input.Substring(4),
                    SteamId64 => (long.Parse(_input) - ChangeVal).ToString(),
                    _ => Id32
                };
            }

            return Id32;
        }

        private string CalcSteamId64()
        {
            if (_inputType == SteamId64)
                Id64 = _input;
            else
                Id64 = _inputType switch
                {
                    SteamId => (int.Parse(_input.Substring(10)) * 2 + int.Parse($"{_input[8]}") + ChangeVal).ToString(),
                    SteamId3 => (int.Parse(_input.Substring(4)) + ChangeVal).ToString(),
                    SteamId32 => (int.Parse(_input) + ChangeVal).ToString(),
                    _ => Id64
                };

            return Id64;
        }


        public void ConvertAll()
        {
            CalcSteamId();
            CalcSteamId3();
            CalcSteamId32();
            CalcSteamId64();
        }

        public string PrintAll()
        {
            return $"SteamID: {Id}\nSteamID3: {Id3}\nSteamID32: {Id32}\nSteamID64: {Id64}";
        }

        private class SteamIdConvertException : Exception
        {
            public SteamIdConvertException(string message) : base(message)
            {
            }
        }
    }
}
