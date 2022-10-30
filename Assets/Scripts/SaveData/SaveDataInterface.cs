using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UniRx.Async;

public abstract class SaveDataInterface<T> : ScriptableObject where T : class
{
    [SerializeField] string saveDir;
    [SerializeField] string saveName;

    public bool available {get; private set;} = false;

    const string saltStr = "eSGiFgNkSogHnsF0fREn";

    const string keyStr = "MdZ8Ukibmw35AkRH7T9A";

    public async UniTask<UniRx.Unit> SaveAsync(T data)
    {
        return await UniTask.Run<UniRx.Unit>(dat=>{Save((T)dat); return UniRx.Unit.Default;}, data);
    }

    public async UniTask<UniRx.Unit> LoadAsync(T data)
    {
        return await UniTask.Run<UniRx.Unit>(dat=>{Load((T)dat); return UniRx.Unit.Default;}, data);
    }

    public void Save(in T data)
    {
        string json = JsonUtility.ToJson(data);
        string path = SavePath;
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        (byte[] cipherJsonWithToken, byte[] checkTokenPlain, byte[] iv) = Encrypt(json, keyStr);

        using(var fs = new FileStream(path, FileMode.Create))
        using(var bw = new BinaryWriter(fs))
        {
            bw.Write(cipherJsonWithToken.Length);
            bw.Write(checkTokenPlain);
            bw.Write(cipherJsonWithToken);
        }

        Debug.Log($"{saveName} was successfully saved in {path}.");
    }

    public void Load(T outdata)
    {
        string path = SavePath;
        string maybeJson;

        using(var fs = new FileStream(path, FileMode.Open))
        using(var br = new BinaryReader(fs))
        {
            int cipherLength = br.ReadInt32();
            System.UInt32 checkTokenPlain = TokenBytesToUInt32(br.ReadBytes(16)); 
            byte[] cipherMaybeJsonWithToken = br.ReadBytes(cipherLength);

            maybeJson = Decrypt(cipherMaybeJsonWithToken, checkTokenPlain, keyStr);
        }
        JsonUtility.FromJsonOverwrite(maybeJson, outdata);
        
        Debug.Log($"{saveName} was successfully loaded from {path}.");
    }

    public string SavePath {get => Path.Combine(Application.persistentDataPath, saveDir, $"{saveName}.savedata");}

    (byte[] cipher, byte[] checkTokenPlain, byte[] iv) 
        Encrypt(string plain, string password)
    {
        var aes = Aes.Create();
        (byte[] key, byte[] iv) = GenerateKeyAndIv(password, aes.KeySize, aes.BlockSize);
        aes.Key = key;
        aes.IV = iv;

        byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
        byte[] plainBytesWithToken = new byte[plainBytes.Length+16];
        byte[] checkToken = GetTokenBytes();
        System.Array.Copy(plainBytes, plainBytesWithToken, plainBytes.Length);
        System.Array.Copy(checkToken, 0, plainBytesWithToken, plainBytes.Length, checkToken.Length);

        byte[] cipherBytes;
        using(ICryptoTransform encryptor = aes.CreateEncryptor())
        {
            cipherBytes = encryptor.TransformFinalBlock(plainBytesWithToken, 0, plainBytesWithToken.Length);
        }

        return (cipherBytes, checkToken, iv);
    }
    
    string Decrypt(byte[] cipher, System.UInt32 checkTokenPlain, string password)
    {
        var aes = Aes.Create();
        (byte[] key, byte[] iv) = GenerateKeyAndIv(password, aes.KeySize, aes.BlockSize);
        aes.Key = key;
        aes.IV = iv;

        byte[] plainBytesWithToken;
        using(ICryptoTransform decryptor = aes.CreateDecryptor())
        {
            plainBytesWithToken = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            System.UInt32 checkTokenDecrypted = TokenBytesToUInt32(plainBytesWithToken, plainBytesWithToken.Length-16);
            
            if(checkTokenPlain != checkTokenDecrypted)
            {
                throw new System.ArgumentException($"Failed to decrypt. Wrong checkToken or password may be given. {checkTokenPlain:x} {checkTokenDecrypted:x}");
            }

        }

        return Encoding.UTF8.GetString(plainBytesWithToken, 0, plainBytesWithToken.Length-16);
    }

    (byte[] key, byte[] iv) GenerateKeyAndIv(string password, int keySize, int blockSize)
    {
        byte[] salt = Encoding.UTF8.GetBytes(saltStr);
        var deriveBytes = new Rfc2898DeriveBytes(password, salt, 1000);
        return (deriveBytes.GetBytes(keySize/8), deriveBytes.GetBytes(blockSize/8));
    }

    byte[] GetTokenBytes()
    {
        XorShift xorShift = new XorShift((System.UInt32)System.Environment.TickCount);
        System.UInt32 rawtoken = xorShift.GetRand();
        byte[] checkToken = new byte[16];
        checkToken[0] = (byte)((rawtoken >> 24) & 0x0000_00ff);
        checkToken[1] = (byte)((rawtoken >> 16) & 0x0000_00ff);
        checkToken[2] = (byte)((rawtoken >> 8) & 0x0000_00ff);
        checkToken[3] = (byte)((rawtoken) & 0x0000_00ff);

        return checkToken;
    }

    System.UInt32 TokenBytesToUInt32(byte[] tokenBytes, int index = 0)
    {
        return
            (((System.UInt32)tokenBytes[index + 0]) << 24) +
            (((System.UInt32)tokenBytes[index + 1]) << 16) +
            (((System.UInt32)tokenBytes[index + 2]) << 8) +
            (((System.UInt32)tokenBytes[index + 3]));
    }
}
