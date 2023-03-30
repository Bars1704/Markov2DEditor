# UnityMarkov

## What is it?
UnityMarkov is an Unity tool for procedural generation, based on [Markov algorithms](https://en.wikipedia.org/wiki/Markov_algorithm)

## Markov algorithms
A Markov algorithm over an alphabet `A` is an ordered list of rules. Each rule is a string of the form `x=y`, where `x` and `y` are words in `A`, and some rules may be marked as halt rules. Application of a Markov algorithm to a word `w` proceeds as follows:
1. Find the first rule `x=y` where `x` is a substring of `w`. If there are no such rules, then halt.
2. Replace the leftmost `x` in `w` by `y`.
3. If the found rule was a halt rule, then halt. Otherwise, go to step 1.

For example, consider this Markov algorithm in the alphabet `{0, 1, x}` (ε is the empty word):
```
1=0x
x0=0xx
0=ε
```
If we apply it to the string `110` we get this sequence of strings:
```
110 -> 0x10 -> 0x0x0 -> 00xxx0 -> 00xx0xx -> 00x0xxxx -> 000xxxxxx -> 00xxxxxx -> 0xxxxxx -> xxxxxx
```

Scale this on several dimensions, add visualisation (and, progably, some magic), and...

![image](https://user-images.githubusercontent.com/33464332/228719819-bc2a43d2-53de-427f-bf36-eff476191b03.png)
![image](https://user-images.githubusercontent.com/33464332/228719953-e786fa1a-2bf9-454c-973c-175b880d200d.png)

voila! Simple maze just in several rules - and that`s only beginning

## Advantages
* Versatility - generation core writen on C# without any Unity libraries - you can use it where you want it! Fore core source code, see [Markokv](github.com/Bars1704/UnityMarkov)
* Scalability - you can write your own nodes, if you need - all you have to do is write node class and it`s drawer!
* Discreteness - using your simulation and having only generation seed you can reproduce same results every time you need it!
*
