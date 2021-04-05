﻿namespace CafeLib.Bitcoin.Global
{
    public class KzStnParams : KzChainParams
    {
        public KzStnParams() : base()
        {
            strNetworkID = "stn";

            base58Prefixes[(int)KzBase58Type.PUBKEY_ADDRESS] = new byte[] { (111) };
            base58Prefixes[(int)KzBase58Type.SCRIPT_ADDRESS] = new byte[] { (196) };
            base58Prefixes[(int)KzBase58Type.SECRET_KEY] = new byte[] { (239) };
            base58Prefixes[(int)KzBase58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x35), (0x87), (0xCF) };
            base58Prefixes[(int)KzBase58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x35), (0x83), (0x94) };
        }
    }
}