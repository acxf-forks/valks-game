## Setup
Current project Unity version `2020.2.1f1`, if there is a newer version of Unity please inform me.

## Coding Style
### Generic
- Methods should follow PascalFormat
- Try to keep accessor and mutator methods at bottom of classes.
### Comments
Start comments with `/*!` so Doxygen can recognize them.
```cs
/*!
 * This is a function.
 * 
 * @return Returns this.
 */
```

## Creating a Pull Request
1. Always test the application to see if it works as intended with no additional bugs you may be adding!
2. State all the changes you made in the PR, not everyone will understand what you've done!

## Generating Documentation
Make sure [Doxygen](https://www.doxygen.nl/index.html) is installed and added to your environment path.

Run `.github/UPDATE-DOCUMENTATION.cmd` after all your initial commits then push.

## Documentation
Documentation can be found [here](https://valks-games.github.io/valks-game/html/index.html).
