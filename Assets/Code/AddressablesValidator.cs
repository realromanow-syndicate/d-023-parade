using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class AddressablesValidator : MonoBehaviour
{
    [SerializeField] string assetAddress; // Адрес любого из ваших ассетов
    
    void Start()
    {
        StartCoroutine(ValidateAddressables());
    }
    
    IEnumerator ValidateAddressables()
    {
        Debug.Log("Инициализация Addressables...");
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;
        
        Debug.Log("Проверка доступности каталога...");
        var catalogHandle = Addressables.CheckForCatalogUpdates(false);
        yield return catalogHandle;
        
        var catalogs = catalogHandle.Result;
        Debug.Log("Найдено каталогов: " + catalogs.Count);
        
        if (!string.IsNullOrEmpty(assetAddress))
        {
            Debug.Log("Проверка загрузки ассета по адресу: " + assetAddress);
            var loadHandle = Addressables.LoadAssetAsync<Object>(assetAddress);
            yield return loadHandle;
            
            if (loadHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Ошибка загрузки ассета: " + loadHandle.OperationException);
            }
            else
            {
                Debug.Log("Ассет успешно загружен: " + loadHandle.Result.name);
                Addressables.Release(loadHandle);
            }
        }
    }
}
