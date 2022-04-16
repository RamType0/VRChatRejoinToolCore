using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace VRChatRejoinToolCore
{
    internal class LogParser
    {
        public static void Add(List<Visit> visits,ReadOnlySpan<byte> log)
        {
            var remaining = log;
            var instanceTokenStartIndex = DestinationSetInstanceIdIndexOf(remaining);
            Span<char> timeStampParseBuffer = stackalloc char[19];
            while(instanceTokenStartIndex >= 0)
            {
                var instanceTokenLength = remaining[instanceTokenStartIndex..].IndexOf((byte)'\n');
                var instanceToken = Encoding.UTF8.GetString(remaining.Slice(instanceTokenStartIndex, instanceTokenLength));

                var previousLineEndIndex = remaining[..instanceTokenStartIndex].LastIndexOf((byte)'\n');
                var timeStampStartIndex = previousLineEndIndex >= 0 ? previousLineEndIndex + 1 : 0;
                var timeStampSpan = remaining.Slice(timeStampStartIndex, 19);

                //Utf8Parser.TryParse(timeStampSpan, out DateTime timeStamp, out _);
                Utf8.ToUtf16(timeStampSpan, timeStampParseBuffer, out _, out _);
                var timeStamp = DateTime.Parse(timeStampParseBuffer);

                remaining = remaining[(instanceTokenStartIndex + instanceToken.Length)..];

                var nextInstanceIdStartIndex = DestinationSetInstanceIdIndexOf(remaining);
                var worldNameSearchLength = nextInstanceIdStartIndex >= 0 ? nextInstanceIdStartIndex : remaining.Length;
                var worldNameSearchSpan = remaining[..worldNameSearchLength];
                var worldName = ExtractWorldName(worldNameSearchSpan);

                visits.Add(new Visit(new Instance(instanceToken, worldName), timeStamp));

                instanceTokenStartIndex = nextInstanceIdStartIndex;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int DestinationSetInstanceIdIndexOf(ReadOnlySpan<byte> logFragment)
        {
            ReadOnlySpan<byte> destinationSet = stackalloc byte[] {
                (byte)'[',
                (byte)'B',
                (byte)'e',
                (byte)'h',
                (byte)'a',
                (byte)'v',
                (byte)'i',
                (byte)'o',
                (byte)'u',
                (byte)'r',
                (byte)']',
                (byte)' ',
                (byte)'D',
                (byte)'e',
                (byte)'s',
                (byte)'t',
                (byte)'i',
                (byte)'n',
                (byte)'a',
                (byte)'t',
                (byte)'i',
                (byte)'o',
                (byte)'n',
                (byte)' ',
                (byte)'s',
                (byte)'e',
                (byte)'t',
                (byte)':',
                (byte)' ',
                (byte)'w',
                (byte)'r',
                (byte)'l',
                (byte)'d',
            };
            var scanedBytes = 0;
            var remaining = logFragment;
            while (true)
            {
                var underbarIndex = remaining.IndexOf((byte)'_'); // '_' is the best single char for searching "wrld_"
                if (underbarIndex >= 0)
                {
                    if (remaining[..underbarIndex].EndsWith(destinationSet))
                    {
                        var localStartIndex = underbarIndex - 4;
                        return scanedBytes + localStartIndex;
                    }
                    else
                    {
                        scanedBytes += underbarIndex + 1;
                        remaining = logFragment[scanedBytes..];
                    }

                }
                else
                {
                    return -1;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string? ExtractWorldName(ReadOnlySpan<byte> logFragment)
        {
            ReadOnlySpan<byte> enteringRoom = stackalloc byte[]
            {
                (byte)'E',
                (byte)'n',
                (byte)'t',
                (byte)'e',
                (byte)'r',
                (byte)'i',
                (byte)'n',
                (byte)'g',
                (byte)' ',
                (byte)'R',
                (byte)'o',
                (byte)'o',
                (byte)'m',
                (byte)':',
                (byte)' ',

            };
            var enteringRoomStartIndex = logFragment.IndexOf(enteringRoom);
            if(enteringRoomStartIndex >= 0)
            {
                var worldNameStartIndex = enteringRoomStartIndex + enteringRoom.Length;
                var worldNameLength = logFragment[worldNameStartIndex..].IndexOf((byte)'\n');
                var worldName = Encoding.UTF8.GetString(logFragment.Slice(worldNameStartIndex, worldNameLength));
                return worldName;
            }
            else
            {
                return null;
            }
        }
    }
}
