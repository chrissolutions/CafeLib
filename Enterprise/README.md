# CafeLib.Secp256k1

CafeLib.Secp256k1 incorporates the source code from [Secp256k1.Net](https://github.com/MeadowSuite/secp256k1) library.  Secp256k1.Net is the cross platform C# wrapper for Cross platform C# wrapper for the native C++ [secp256k1 library](https://github.com/MeadowSuite/secp256k1/blob/master/Secp256k1.Native.nuspec).

CafeLib.BsvSharp is heavily based on KzBsv and uses Secp256k1.Net in the same manner to provided the cryptographic algorithms for the production of keys.  The main alteration has to do with the nuget packaging process to properly construct the nuget package to contain the native secp256k1 libraries.

The nuget package supports win-x64, win-x86, macOS-x64, and linux-x64 out of the box. The native libraries are bundled from the [Secp256k1.Native package](https://www.nuget.org/packages/Secp256k1.Native/). This wrapper should work on any other platform that supports netstandard2.1 but requires that the [native secp256k1](https://github.com/MeadowSuite/secp256k1) library be compiled from source. 

------

### Example Usage

```csharp
// Create a secp256k1 context (ensure disposal to prevent unmanaged memory leaks).
using (var secp256k1 = new CafeLib.Secp256k1())
{

    // Generate a private key.
    var privateKey = new byte[32];
    var rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
    do { rnd.GetBytes(privateKey); }
    while (!secp256k1.SecretKeyVerify(privateKey));


    // Create public key from private key.
    var publicKey = new byte[64];
    Debug.Assert(secp256k1.PublicKeyCreate(publicKey, privateKey));


    // Sign a message hash.
    var messageBytes = Encoding.UTF8.GetBytes("Hello world.");
    var messageHash = System.Security.Cryptography.SHA256.Create().ComputeHash(messageBytes);
    var signature = new byte[64];
    Debug.Assert(secp256k1.Sign(signature, messageHash, privateKey));


    // Verify message hash.
    Debug.Assert(secp256k1.Verify(signature, messageHash, publicKey));

}
```

