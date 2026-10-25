using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace firewood;

public class InteractManager
{
    private KeyboardState _state;

    public void Update()
    {
        _state = Keyboard.GetState(); 
    }

    public bool Interact(Keys key)
    {
        if(_state.IsKeyDown(key) && !GameData.previousState.IsKeyDown(key))
        {
            return true;
        } else {
            return false;
        }
    }
}