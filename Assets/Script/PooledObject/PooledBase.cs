using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PooledBase : MonoBehaviour
{
    protected PooledObject _me_PooledType;
    protected PooledObjectInner _me_PooledInnerType;
}
