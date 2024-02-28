// Delegates // (Ref: https://gamedevbeginner.com/events-and-delegates-in-unity/#event_based_systems )

using System;
using System.Collections.Generic;

delegate void MyDelegate();
MyDelegate attack;
// The attack delegate can now hold any function that matches the “My Delegate” signature.
// We can now assign any function to the attack delegate so long as it has a void return type and no parameters


// How to use delegates with parameters
// The parameter’s value is then passed in when the delegate gets called.
// Any function that’s assigned to the delegate must accept a matching set of parameters
public delegate void OnGameOver(bool canRetry);


// Assigning a function
delegate void MyDelegate();
MyDelegate attack;

void Start()
{
    attack = PrimaryAttack;
}

void PrimaryAttack()
{
    // Attack!
}

// Now, when the attack delegate is triggered the Primary Attack function will be called
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        attack();
    }
}

// It’s important to only actually call a delegate if there’s a function assigned to it, as trying to trigger an empty delegate will cause a null reference error.
attack?.Invoke();


// we can now change the attack function to, instead, trigger a secondary attack by using the number keys to switch attack types
public class DelegateExample : MonoBehaviour
{
    delegate void MyDelegate();
    MyDelegate attack;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (attack != null)
            {
                attack();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            attack = PrimaryAttack;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            attack = SecondaryAttack;
        }
    }

    void PrimaryAttack()
    {
        // Primary attack
    }

    void SecondaryAttack()
    {
        // Secondary attack
    }

}



// multicast  trigger - trigger multiple functions all at once
delegate void MyDelegate();
MyDelegate attack;
void Start()
{
    attack += PrimaryAttack;
    attack += SecondaryAttack;
}

// to call multiple functions from a delegate can also be useful for creating an events system between multiple scripts.
public class PlayerHealth : MonoBehaviour
{
    float health = 100;

    delegate void OnGameOver();
    OnGameOver onGameOver;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            onGameOver?.Invoke();
        }
    }
}

// In order for other scripts to be able to easily access the delegate, it needs to be public and it will need to be static.
//  Game Controller script to subscribe its RestartGame function to the OnGameOver event, without needing a reference to the player object first.
public delegate void OnGameOver();
public static OnGameOver onGameOver;

public class GameController : MonoBehaviour
{
    void RestartGame()
    {
        // Restart the game!
    }

    private void OnEnable()
    {
        PlayerHealth.onGameOver += RestartGame;
    }
}

// it’s extremely important to unsubscribe the function if it’s no longer needed.
// The simplest way to do this is to also subtract the function from the delegate if the controller object is ever disabled, in the On Disable event
public class GameController : MonoBehaviour
{
    void RestartGame()
    {
        // Restart the game!
    }

    private void OnEnable()
    {
        PlayerHealth.onGameOver += RestartGame;
    }

    private void OnDisable()
    {
        PlayerHealth.onGameOver -= RestartGame;
    }
}

// one problem
// any other script can access the delegate, it’s technically possible for any script to call it or even clear the assigned functions from the delegate
// to prevent this, use the event keyword


// Events //

// while delegates can be called by other scripts, event delegates can only be triggered from within their own class
// scripts can subscribe and unsubscribe their own functions but they cannot trigger the event, or alter the subscription of other functions from other classes.
public delegate void OnGameOver();
public static event OnGameOver onGameOver

// it’s possible to use a delegate as a parameter in a method,
// which would allow you to pass a function into a function, which could be useful for executing a different command after something has happened.
// Whereas events, unsurprisingly, work well as part of an events system, where one object’s actions trigger other scripts to respond
// events are more suited to this particular task of triggering an event that other, observing objects can then respond to but not interfere with.

// Actions //

// ready-made delegates.
// They can be used to easily create a delegate or delegate event with a void return type.
using System;
public static event Action OnGameOver;

public static event Action<string> OnGameOver;
public static event Action<float, bool> OnPlayerHurt;


// Uinty Events //

// Unity Events work in a similar way to event delegates, in that they can be triggered from a script allowing other, observing scripts and components to act in response.
// difference is, while event delegates are typically managed in scripting, Unity Events are serialisable, meaning that you can work with them in the Inspector.
// Using Unity Events to manage the interactions between scripts can encourage you to limit the scope of each script to a singular purpose.
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    float health = 100;
    public UnityEvent onPlayerDeath;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            onPlayerDeath.Invoke();
        }
    }
}
// no need of using null check - while Unity Events act like delegates, they’re technically a Unity class.
// Which means that trying to call one when it’s empty won’t cause you an error.

// create a custom dynamic value Unity Event - create a separate serializable class
using UnityEngine.Events;
using System;

[Serializable]
public class FloatEvent : UnityEvent<float> { }
//  this particular version of Unity Event takes a generic parameter which, in this case float,
//  by adding “float” in angled brackets, after the class declaration, but before the class body, which is empty

// Now, you can use this new Float Event class as a type for a Unity Event in your script
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    float health = 100;
    public UnityEvent onPlayerDeath;
    public FloatEvent onPlayerHurt;

    public void TakeDamage(float damage)
    {
        health -= damage;
        onPlayerHurt.Invoke(damage);
        if (health < 0)
        {
            onPlayerDeath.Invoke();
        }
    }
}

// Scriptable Object Unity Events // - to combine the convenience of Unity Events with the advantages of game-wide event delegates

// two unrelated game objects can react to the same event without needing to know about each other

// scriptable objects? - when you create a scriptable object, you’re really defining a template from which you can create unique copies,
// where each copy is an instance of the scriptable object type, with its own data and that can be saved as an asset. 
// advantage of scriptable objects is that they can be used to create global variables of a specific type, including global game events

// How it works? - need two scripts : scriptable object Game Event and Monobehaviour Game Event Listener

// Game Event - scriptable object
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners = new List<GameEventListener>();

    public void TriggerEvent()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventTriggered();
        }
    }

    public void AddListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}

// Game Event Listener 
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent onEventTriggered;

    void OnEnable()
    {
        gameEvent.AddListener(this);
    }

    void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }

    public void OnEventTriggered()
    {
        onEventTriggered.Invoke();
    }
}


// Game Event is the subject. It keeps a list of observers that are interested in the event and will trigger all of them if its event is called.

// Game Event Listener is the observer. It listens for the event and triggers its own Unity Event when the event is called.
// hold a reference to a global Game Event, which it uses to subscribe its own trigger function to that event, so that it can be called by the subject when the event takes place.

// idea is that the subject and the observer both share the same global Game Event variable, connecting the two
// can help to limit each script to a single purpose