using Unity.Entities;
using UnityEngine;

public partial class InputSystem : SystemBase
{
    private Player _player;
    protected override void OnCreate()
    {
        if (!SystemAPI.TryGetSingleton(out InputComponent input)){
            EntityManager.CreateEntity(typeof(InputComponent));
        }

        _player = new Player();
        _player.Enable();
    }
    protected override void OnUpdate()
    {
        Vector2 moveVector = _player.PlayerMap.Move.ReadValue<Vector2>();
        Vector2 mousePos = _player.PlayerMap.MousePos.ReadValue<Vector2>();
        bool Shoot = _player.PlayerMap.Shoot.IsPressed();

        SystemAPI.SetSingleton(new InputComponent{

            MousePos = mousePos,
            Movement = moveVector,
            Shoot = Shoot
        });
    }
}
