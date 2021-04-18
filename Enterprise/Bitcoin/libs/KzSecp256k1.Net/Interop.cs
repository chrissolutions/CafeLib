﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Secp256k1Net
{

    /// <summary>
    /// Create a secp256k1 context object.
    /// </summary>
    /// <param name="flags">which parts of the context to initialize.</param>
    /// <returns>a newly created context object.</returns>
    [SymbolName(nameof(secp256k1_context_create))]
    public delegate IntPtr secp256k1_context_create(uint flags);

    /// <summary>
    /// Updates the context randomization to protect against side-channel leakage.
    ///
    /// While secp256k1 code is written to be constant-time no matter what secret
    /// values are, it's possible that a future compiler may output code which isn't,
    /// and also that the CPU may not emit the same radio frequencies or draw the same
    /// amount power for all values.
    ///
    /// This function provides a seed which is combined into the blinding value: that
    /// blinding value is added before each multiplication (and removed afterwards) so
    /// that it does not affect function results, but shields against attacks which
    /// rely on any input-dependent behaviour.
    ///
    /// You should call this after secp256k1_context_create or
    /// secp256k1_context_clone, and may call this repeatedly afterwards.
    /// </summary>
    /// <param name="ctx">pointer to a context object (cannot be NULL)</param>
    /// <param name="seed32">pointer to a 32-byte random seed (NULL resets to initial state)</param>
    /// <returns>
    /// 1: randomization successfully updated
    /// 0: error
    /// </returns>
    [SymbolName(nameof(secp256k1_context_randomize))]
    public unsafe delegate int secp256k1_context_randomize(IntPtr ctx, void* seed32);

    /// <summary>
    /// Destroy a secp256k1 context object. The context pointer may not be used afterwards.
    /// </summary>
    /// <param name="ctx">ctx: an existing context to destroy (cannot be NULL).</param>
    [SymbolName(nameof(secp256k1_context_destroy))]
    public delegate void secp256k1_context_destroy(IntPtr ctx);

    /// <summary>
    /// Tweak a private key by adding tweak to it.
    /// </summary>
    /// <param name="ctx">pointer to a context object, initialized for signing (cannot be NULL)</param>
    /// <param name="seckey">In/Out:  seckey: pointer to a 32-byte private key.</param>
    /// <param name="tweak">In:      tweak:  pointer to a 32-byte tweak.</param>
    /// <returns>
    /// 0 if the tweak was out of range (chance of around 1 in 2^128 for uniformly random 32-byte arrays, or if the resulting private key
    /// would be invalid (only when the tweak is the complement of the private key). 1 otherwise.
    /// </returns>
    [SymbolName(nameof(secp256k1_ec_privkey_tweak_add))]
    public unsafe delegate int secp256k1_ec_privkey_tweak_add(IntPtr ctx,
        void* seckey,       // unsigned char* seckey,
        void* tweak         // const unsigned char* tweak
    );

    /// <summary>
    /// Tweak a public key by adding tweak times the generator to it.
    /// </summary>
    /// <param name="ctx">pointer to a context object, initialized for signing (cannot be NULL)</param>
    /// <param name="pubkey">In/Out:  pubkey: pointer to a public key object.</param>
    /// <param name="tweak">In:      tweak:  pointer to a 32-byte tweak.</param>
    /// <returns>
    /// 0 if the tweak was out of range (chance of around 1 in 2^128 for uniformly random 32-byte arrays, or if the resulting private key
    /// would be invalid (only when the tweak is the complement of the private key). 1 otherwise.
    /// </returns>
    [SymbolName(nameof(secp256k1_ec_pubkey_tweak_add))]
    public unsafe delegate int secp256k1_ec_pubkey_tweak_add(IntPtr ctx,
        void* pubkey,       // secp256k1_pubkey* pubkey,
        void* tweak         // const unsigned char* tweak
    );

    /// <summary>
    /// Tweak a private key by multiplying it by a tweak.
    /// </summary>
    /// <param name="ctx">pointer to a context object, initialized for signing (cannot be NULL)</param>
    /// <param name="seckey">In/Out:  seckey: pointer to a 32-byte private key.</param>
    /// <param name="tweak">In:      tweak:  pointer to a 32-byte tweak.</param>
    /// <returns>
    /// 0 if the tweak was out of range (chance of around 1 in 2^128 for uniformly random 32-byte arrays, or if the resulting private key
    /// would be invalid (only when the tweak is the complement of the private key). 1 otherwise.
    /// </returns>
    [SymbolName(nameof(secp256k1_ec_privkey_tweak_mul))]
    public unsafe delegate int secp256k1_ec_privkey_tweak_mul(IntPtr ctx,
        void* seckey,       // unsigned char* seckey,
        void* tweak         // const unsigned char* tweak
    );

    /// <summary>
    /// Tweak a public key by multiplying it by a tweak value.
    /// </summary>
    /// <param name="ctx">pointer to a context object, initialized for signing (cannot be NULL)</param>
    /// <param name="pubkey">In/Out:  pubkey: pointer to a public key object.</param>
    /// <param name="tweak">In:      tweak:  pointer to a 32-byte tweak.</param>
    /// <returns>
    /// 0 if the tweak was out of range (chance of around 1 in 2^128 for uniformly random 32-byte arrays, or if the resulting private key
    /// would be invalid (only when the tweak is the complement of the private key). 1 otherwise.
    /// </returns>
    [SymbolName(nameof(secp256k1_ec_pubkey_tweak_mul))]
    public unsafe delegate int secp256k1_ec_pubkey_tweak_mul(IntPtr ctx,
        void* pubkey,       // secp256k1_pubkey* pubkey,
        void* tweak         // const unsigned char* tweak
    );

    /// <summary>
    /// Create a recoverable ECDSA signature.
    /// </summary>
    /// <param name="ctx">pointer to a context object, initialized for signing (cannot be NULL)</param>
    /// <param name="sig">(Output) pointer to an array where the signature will be placed (cannot be NULL)</param>
    /// <param name="msg32">the 32-byte message hash being signed (cannot be NULL)</param>
    /// <param name="seckey">pointer to a 32-byte secret key (cannot be NULL)</param>
    /// <param name="noncefp">pointer to a nonce generation function. If NULL, secp256k1_nonce_function_default is used</param>
    /// <param name="ndata">pointer to arbitrary data used by the nonce generation function (can be NULL)</param>
    /// <returns>
    /// 1: signature created
    /// 0: the nonce generation function failed, or the private key was invalid.
    /// </returns>
    [SymbolName(nameof(secp256k1_ecdsa_sign_recoverable))]
    public unsafe delegate int secp256k1_ecdsa_sign_recoverable(IntPtr ctx,
        void* sig,      // secp256k1_ecdsa_recoverable_signature *sig 
        void* msg32,    // const unsigned char* msg32
        void* seckey,   // const unsigned char* seckey
        IntPtr noncefp, // secp256k1_nonce_function noncefp
        IntPtr ndata    // const void* ndata
    );

    /// <summary>
    /// Obtains the public key for a given private key.
    /// </summary>
    /// <param name="ctx">pointer to a context object, initialized for signing (cannot be NULL)</param>
    /// <param name="pubKeyOut">(Output) pointer to the created public key (cannot be NULL)</param>
    /// <param name="privKeyIn">(Input) pointer to a 32-byte private key (cannot be NULL)</param>
    /// <returns>
    /// 1: secret was valid, public key stores
    /// 0: secret was invalid, try again
    /// </returns>
    [SymbolName(nameof(secp256k1_ec_pubkey_create))]
    public unsafe delegate int secp256k1_ec_pubkey_create(IntPtr ctx,
        void* pubKeyOut,    // secp256k1_pubkey *pubkey,
        void* privKeyIn     // const unsigned char *seckey
    );

    /// <summary>
    /// Parse a variable-length public key into the pubkey object.
    /// This function supports parsing compressed (33 bytes, header byte 0x02 or
    /// 0x03), uncompressed(65 bytes, header byte 0x04), or hybrid(65 bytes, header
    /// byte 0x06 or 0x07) format public keys.
    /// </summary>
    /// <param name="ctx">a secp256k1 context object.</param>
    /// <param name="pubkey">(Output) pointer to a pubkey object. If 1 is returned, it is set to a parsed version of input. If not, its value is undefined.</param>
    /// <param name="input">pointer to a serialized public key.</param>
    /// <param name="inputlen">length of the array pointed to by input</param>
    /// <returns>1 if the public key was fully valid, 0 if the public key could not be parsed or is invalid.</returns>
    [SymbolName(nameof(secp256k1_ec_pubkey_parse))]
    public unsafe delegate int secp256k1_ec_pubkey_parse(IntPtr ctx,
        void* pubkey,   // secp256k1_pubkey* pubkey,  
        void* input,    // const unsigned char* input,
        uint inputlen   // size_t inputlen
    );

    /// <summary>
    /// Serialize a pubkey object into a serialized byte sequence.
    /// </summary>
    /// <param name="ctx">a secp256k1 context object.</param>
    /// <param name="output">a pointer to a 65-byte (if compressed==0) or 33-byte (if compressed==1) byte array to place the serialized key in.</param>
    /// <param name="outputlen">a pointer to an integer which is initially set to the size of output, and is overwritten with the written size.</param>
    /// <param name="pubkey">a pointer to a secp256k1_pubkey containing an initialized public key.</param>
    /// <param name="flags">SECP256K1_EC_COMPRESSED if serialization should be in compressed format, otherwise SECP256K1_EC_UNCOMPRESSED.</param>
    /// <returns>1 always</returns>
    [SymbolName(nameof(secp256k1_ec_pubkey_serialize))]
    public unsafe delegate int secp256k1_ec_pubkey_serialize(IntPtr ctx,
        void* output,       // unsigned char* output
        ref uint outputlen, // size_t *outputlen
        void* pubkey,       // const secp256k1_pubkey* pubkey
        uint flags          // unsigned int flags
    );

    /// <summary>
    /// Verify an ECDSA secret key.
    /// </summary>
    /// <param name="ctx">a secp256k1 context object.</param>
    /// <param name="seckey">Pointer to a 32-byte secret key.</param>
    /// <returns>1 if secret key is valid, 0 if secret key is invalid.</returns>
    [SymbolName(nameof(secp256k1_ec_seckey_verify))]
    public unsafe delegate int secp256k1_ec_seckey_verify(IntPtr ctx,
        void* seckey // const unsigned char* seckey
    );

#if false
    /// <summary>
    /// Parse a signature in "lax DER" format
    ///
    /// This function will accept any valid DER encoded signature, even if the
    /// encoded numbers are out of range. In addition, it will accept signatures
    /// which violate the DER spec in various ways. Its purpose is to allow
    /// validation of the Bitcoin blockchain, which includes non-DER signatures
    /// from before the network rules were updated to enforce DER. Note that
    /// the set of supported violations is a strict subset of what OpenSSL will
    /// accept.
    ///
    /// After the call, sig will always be initialized. If parsing failed or the
    /// encoded numbers are out of range, signature validation with it is
    /// guaranteed to fail for every message and public key.
    /// </summary>
    /// <param name="ctx">a secp256k1 context object</param>
    /// <param name="sig">Out: a pointer to a signature object</param>
    /// <param name="input">In: a pointer to the signature to be parsed</param>
    /// <param name="inputlen">In: the length of the array pointed to be input</param>
    /// <returns>Returns: 1 when the signature could be parsed, 0 otherwise.</returns>
    /// Not currently exported by C++ DLL.
    [SymbolName(nameof(ecdsa_signature_parse_der_lax))]
    public unsafe delegate int ecdsa_signature_parse_der_lax(IntPtr ctx,
        void* sig, // secp256k1_ecdsa_signature
        void* input, // const unsigned char
        uint inputlen // size_t 
    );
#endif

    /// <summary>
    /// Normalizes a signature and enforces a low-S.
    /// </summary>
    /// <param name="ctx">pointer to a context object, initialized for signing (cannot be NULL)</param>
    /// <param name="sigout">(Output) pointer to an array where the normalized signature will be placed (cannot be NULL)</param>
    /// <param name="sigin">(Input) pointer to an array where a signature to normalize resides (cannot be NULL)</param>
    /// <returns>1: correct signature, 0: incorrect or unparseable signature</returns>
    [SymbolName(nameof(secp256k1_ecdsa_signature_normalize))]
    public unsafe delegate int secp256k1_ecdsa_signature_normalize(IntPtr ctx,
        void* sigout,   // secp256k1_ecdsa_signature* sigout
        void* sigin     // const secp256k1_ecdsa_signature* sigin
    );

    /// <summary>
    /// Serialize an ECDSA signature in DER format.
    /// </summary>
    /// <param name="ctx">a secp256k1 context object</param>
    /// <param name="output">a pointer to an array to store the DER serialization</param>
    /// <param name="outputlen">a pointer to a length integer. Initially, this integer should be set to the length of output.
    /// After the call it will be set to the length of the serialization (even if 0 was returned).
    /// </param>
    /// <param name="sig">a pointer to an initialized signature object</param>
    /// <returns>1 if enough space was available to serialize, 0 otherwise</returns>
    [SymbolName(nameof(secp256k1_ecdsa_signature_serialize_der))]
    public unsafe delegate int secp256k1_ecdsa_signature_serialize_der(IntPtr ctx,
        void* output,       // unsigned char* output,
        ref int outputlen,    // size_t* outputlen,
        void* sig           // const secp256k1_ecdsa_signature* sig
    );
    
    /// <summary>
    /// Serialize an ECDSA signature in compact format (64 bytes + recovery id).
    /// </summary>
    /// <param name="ctx">a secp256k1 context object</param>
    /// <param name="output64">(Output) a pointer to a 64-byte array of the compact signature (cannot be NULL).</param>
    /// <param name="recid">(Output) a pointer to an integer to hold the recovery id (can be NULL).</param>
    /// <param name="sig">a pointer to an initialized signature object (cannot be NULL).</param>
    /// <returns>1 always</returns>
    [SymbolName(nameof(secp256k1_ecdsa_recoverable_signature_serialize_compact))]
    public unsafe delegate int secp256k1_ecdsa_recoverable_signature_serialize_compact(IntPtr ctx,
        void* output64, // unsigned char* output64
        ref int recid,  // int* recid
        void* sig       // const secp256k1_ecdsa_recoverable_signature* sig
    );

    /// <summary>
    /// Recover an ECDSA public key from a signature.
    /// </summary>
    /// <param name="ctx">pointer to a context object, initialized for verification (cannot be NULL)</param>
    /// <param name="pubkey">(Output) pointer to the recovered public key (cannot be NULL)</param>
    /// <param name="sig">pointer to initialized signature that supports pubkey recovery (cannot be NULL)</param>
    /// <param name="msg32">the 32-byte message hash assumed to be signed (cannot be NULL)</param>
    /// <returns>
    /// 1: public key successfully recovered (which guarantees a correct signature).
    /// 0: otherwise.
    /// </returns>
    [SymbolName(nameof(secp256k1_ecdsa_recover))]
    public unsafe delegate int secp256k1_ecdsa_recover(IntPtr ctx,
        void* pubkey,   // secp256k1_pubkey* pubkey
        void* sig,      // const secp256k1_ecdsa_recoverable_signature* sig
        void* msg32     // const unsigned char* msg32
    );

    /// <summary>
    /// Recover an ECDSA public key from a signature.
    /// Parse an ECDSA signature in compact (64 bytes) format.
    ///
    /// The signature must consist of a 32-byte big endian R value, followed by a
    /// 32-byte big endian S value. If R or S fall outside of [0..order-1], the
    /// encoding is invalid. R and S with value 0 are allowed in the encoding.
    ///
    /// After the call, sig will always be initialized. If parsing failed or R or
    /// S are zero, the resulting sig value is guaranteed to fail validation for any
    /// message and public key.
    /// </summary>
    /// <param name="ctx">a secp256k1 context object</param>
    /// <param name="sig">(Output) a pointer to a signature object</param>
    /// <param name="input64">a pointer to a 64-byte compact signature</param>
    /// <returns>1 when the signature could be parsed, 0 otherwise</returns>
    [SymbolName(nameof(secp256k1_ecdsa_signature_parse_compact))]
    public unsafe delegate int secp256k1_ecdsa_signature_parse_compact(IntPtr ctx,
        void* sig,      // secp256k1_ecdsa_signature* sig
        void* input64  // const unsigned char* input64
    );

    /// <summary>
    /// Parse a compact ECDSA signature (64 bytes + recovery id).
    /// </summary>
    /// <param name="ctx">a secp256k1 context object</param>
    /// <param name="sig">(Output) a pointer to a signature object</param>
    /// <param name="input64">a pointer to a 64-byte compact signature</param>
    /// <param name="recid">the recovery id (0, 1, 2 or 3)</param>
    /// <returns>1 when the signature could be parsed, 0 otherwise</returns>
    [SymbolName(nameof(secp256k1_ecdsa_recoverable_signature_parse_compact))]
    public unsafe delegate int secp256k1_ecdsa_recoverable_signature_parse_compact(IntPtr ctx,
        void* sig,      // secp256k1_ecdsa_recoverable_signature* sig
        void* input64,  // const unsigned char* input64
        int recid       // int recid
    );

    /// <summary>
    /// Verify an ECDSA signature.
    /// To avoid accepting malleable signatures, only ECDSA signatures in lower-S
    /// form are accepted.
    /// If you need to accept ECDSA signatures from sources that do not obey this
    /// rule, apply secp256k1_ecdsa_signature_normalize to the signature prior to
    /// validation, but be aware that doing so results in malleable signatures.
    /// For details, see the comments for that function.
    /// </summary>
    /// <param name="ctx">a secp256k1 context object, initialized for verification.</param>
    /// <param name="sig">the signature being verified (cannot be NULL)</param>
    /// <param name="msg32">the 32-byte message hash being verified (cannot be NULL)</param>
    /// <param name="pubkey">pointer to an initialized public key to verify with (cannot be NULL)</param>
    /// <returns>1: correct signature, 0: incorrect or unparseable signature</returns>
    [SymbolName(nameof(secp256k1_ecdsa_verify))]
    public unsafe delegate int secp256k1_ecdsa_verify(IntPtr ctx,
        void* sig,      // const secp256k1_ecdsa_signature *sig,
        void* msg32,    // const unsigned char *msg32,
        void* pubkey    // const secp256k1_pubkey *pubkey
    );

    /// <summary>
    /// Create an ECDSA signature. The created signature is always in lower-S form. See
    /// secp256k1_ecdsa_signature_normalize for more details.
    /// </summary>
    /// <param name="ctx">Pointer to a context object, initialized for signing (cannot be NULL).</param>
    /// <param name="sig">Pointer to an array where the signature will be placed (cannot be NULL).</param>
    /// <param name="msg32">The 32-byte message hash being signed (cannot be NULL).</param>
    /// <param name="seckey">Pointer to a 32-byte secret key (cannot be NULL).</param>
    /// <param name="noncefp">Pointer to a nonce generation function. If NULL, secp256k1_nonce_function_default is used.</param>
    /// <param name="ndata">Pointer to arbitrary data used by the nonce generation function (can be NULL).</param>
    /// <returns>1: signature created, 0: the nonce generation function failed, or the private key was invalid.</returns>
    [SymbolName(nameof(secp256k1_ecdsa_sign))]
    public unsafe delegate int secp256k1_ecdsa_sign(IntPtr ctx,
        void* sig,      // secp256k1_ecdsa_signature *sig
        void* msg32,    // const unsigned char *msg32
        void* seckey,   // const unsigned char *seckey
        IntPtr noncefp, // secp256k1_nonce_function noncefp
        void* ndata     // const void *ndata
    );

    /// <summary>
    /// Compute an EC Diffie-Hellman secret in constant time.
    /// </summary>
    /// <param name="ctx">Pointer to a context object (cannot be NULL).</param>
    /// <param name="output">Pointer to an array to be filled by the function.</param>
    /// <param name="pubkey">A pointer to a secp256k1_pubkey containing an initialized public key.</param>
    /// <param name="privkey">A 32-byte scalar with which to multiply the point.</param>
    /// <param name="hashfp">Pointer to a hash function. If NULL, secp256k1_ecdh_hash_function_sha256 is used.</param>
    /// <param name="data">Arbitrary data pointer that is passed through.</param>
    /// <returns>1: exponentiation was successful, 0: scalar was invalid(zero or overflow)</returns>
    [SymbolName(nameof(secp256k1_ecdh))]
    public unsafe delegate int secp256k1_ecdh(IntPtr ctx,
        void* output,   // unsigned char *output
        void* pubkey,   // const secp256k1_pubkey *pubkey
        void* privkey,  // const unsigned char *privkey
        IntPtr hashfp,  // secp256k1_ecdh_hash_function hashfp,
        IntPtr data      // void *data
    );

    // Flags copied from
    // https://github.com/bitcoin-core/secp256k1/blob/452d8e4d2a2f9f1b5be6b02e18f1ba102e5ca0b4/include/secp256k1.h#L157

    [Flags]
    public enum Flags : uint
    {
        /** All flags' lower 8 bits indicate what they're for. Do not use directly. */
        SECP256K1_FLAGS_TYPE_MASK = ((1 << 8) - 1),
        SECP256K1_FLAGS_TYPE_CONTEXT = (1 << 0),
        SECP256K1_FLAGS_TYPE_COMPRESSION = (1 << 1),

        /** The higher bits contain the actual data. Do not use directly. */
        SECP256K1_FLAGS_BIT_CONTEXT_VERIFY = (1 << 8),
        SECP256K1_FLAGS_BIT_CONTEXT_SIGN = (1 << 9),
        SECP256K1_FLAGS_BIT_COMPRESSION = (1 << 8),

        /** Flags to pass to secp256k1_context_create. */
        SECP256K1_CONTEXT_VERIFY = (SECP256K1_FLAGS_TYPE_CONTEXT | SECP256K1_FLAGS_BIT_CONTEXT_VERIFY),
        SECP256K1_CONTEXT_SIGN = (SECP256K1_FLAGS_TYPE_CONTEXT | SECP256K1_FLAGS_BIT_CONTEXT_SIGN),
        SECP256K1_CONTEXT_NONE = (SECP256K1_FLAGS_TYPE_CONTEXT),

        /** Flag to pass to secp256k1_ec_pubkey_serialize and secp256k1_ec_privkey_export. */
        SECP256K1_EC_COMPRESSED = (SECP256K1_FLAGS_TYPE_COMPRESSION | SECP256K1_FLAGS_BIT_COMPRESSION),
        SECP256K1_EC_UNCOMPRESSED = (SECP256K1_FLAGS_TYPE_COMPRESSION)
    }


}