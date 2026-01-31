using QFramework;
using UnityEngine;
namespace GGJ2026
{
    public class Stage1ConfirmButton : MonoBehaviour
    {
        public void onClick()
        {
            Debug.Log("请求判定");
            GameApp.Interface.SendCommand(new Stage1ConfirmCommand());
        }
    }
}