## Split %
You will be paid fairly based on your contributions once the project receives enough revenue. Please talk to me about rev-share before you contribute otherwise I will assume you are contributing for free. I do not want to pay for things I could have done myself. Unless you made an honest effort all around in the long run, that's different.

## Setup
Current project Unity version `2020.2.1f1`, if there is a newer version of Unity please inform me.

## Responsibilities
- Update the #change-log with all the changes you made
- Update the Roadmap if you finished something there
- Regularly put clips of your progress in #previews
- Make a habit of commenting code (If I do not understand your code, then it will not be merged)

## Creating a Pull Request
1. Always test the application to see if it works as intended with no additional bugs you may be adding!
2. State all the changes you made in the PR, not everyone will understand what you've done!

## Coding Style
### Generic
- Methods should follow PascalFormat
- Try to keep accessor and mutator methods at bottom of classes.
### Comments
```cs
/*!
 * This is a function.
 * 
 * @return Returns this.
 */
```

## Generating Documentation
Make sure [Doxygen](https://www.doxygen.nl/index.html) is installed and added to your environment path.

Run `.github/UPDATE-DOCUMENTATION.cmd` after all your initial commits then push.

## Documentation
Documentation can be found [here](https://valks-games.github.io/valks-game/html/index.html).
