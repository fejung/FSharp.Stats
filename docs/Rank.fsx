(**
---
title: Ranking
index: 18
category: Documentation
categoryindex: 0
---
*)

(*** hide ***)

(*** condition: prepare ***)
#I "../src/FSharp.Stats/bin/Release/netstandard2.0/"
#r "FSharp.Stats.dll"
#r "nuget: Plotly.NET, 2.0.0-preview.16"

(*** condition: ipynb ***)
#if IPYNB
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.16"
#r "nuget: FSharp.Stats"
#endif // IPYNB


open Plotly.NET
open Plotly.NET.StyleParam
open Plotly.NET.LayoutObjects

//some axis styling
module Chart = 
    let myAxis name = LinearAxis.init(Title=Title.init name,Mirror=StyleParam.Mirror.All,Ticks=StyleParam.TickOptions.Inside,ShowGrid=false,ShowLine=true)
    let myAxisRange name (min,max) = LinearAxis.init(Title=Title.init name,Range=Range.MinMax(min,max),Mirror=StyleParam.Mirror.All,Ticks=StyleParam.TickOptions.Inside,ShowGrid=false,ShowLine=true)
    let withAxisTitles x y chart = 
        chart 
        |> Chart.withTemplate ChartTemplates.lightMirrored
        |> Chart.withXAxis (myAxis x) 
        |> Chart.withYAxis (myAxis y)

(**

# Ranking

[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/fslaborg/FSharp.Stats/gh-pages?filepath=Rank.ipynb)

_Summary:_ this tutorial demonstrates how to determine ranks of a collection

Consider a collection of values. The rank of a number is its size relative to other values in a sequence.
There are four methods how to handle ties:


```
let mySequence = [|1.0; -2.0; 0.0; 1.0|]
```

- **rankFirst**
  - Each rank occurs exactly once. If ties are present the first occurence gets the low rank. 
  - ATTENTION: If there are multiple ties (>20) the sorting of Arrays will not preserve the element order correctly!!
  - `ranks = [3,1,2,4]`


- **rankMin**
  - If ties are present all tied elements receive the score of the first occurence (min rank)
  - `ranks = [3,1,2,3]`

- **rankMax**
  - If ties are present all tied elements receive the score of the last occurence (max rank)
  - `ranks = [4,1,2,4]`


- **rankAverge**
  - If ties are present all tied elements receive their average rank.
  - `ranks = [3.5,1,2,3.5]`


## NaN treatment
If nans are present in the collection, there are several ways to treat them. In general `nan <> nan` so that each occurence will receive its unique rank. 
(for infinity and -infinity the equality check returns true).
  
  - Usually nans are sorted to the beginning of the collection: `nan, -infinity, -100., 0., 100, infinity`
  - By default in FSharp.Stats.Rank, nans are sorted to the end of the sequence (the sorting can be defined as optional parameter)
  - Additionally ranks of nan values are set to nan if not specified othwerwise
*)

open FSharp.Stats

let collection = [|2.;-infinity;infinity;infinity;nan;0|]

Rank.RankFirst() collection
// result:  [|3.0; 1.0; 4.0; 5.0; nan; 2.0|]

Rank.RankMin() collection
// result:  [|3.0; 1.0; 4.0; 4.0; nan; 2.0|]

Rank.RankMax() collection
// result:  [|3.0; 1.0; 5.0; 5.0; nan; 2.0|]

Rank.RankAverage() collection
// result:  [|3.0; 1.0; 4.5; 4.5; nan; 2.0|]

(**


If you want to preserve the true ranks of nans but sort them to the back you can use:

*)

Rank.RankFirst(RankNanWithNan=false) collection
// result:  [|3.0; 1.0; 4.0; 5.0; 6.0; 2.0|]


(**


If you want to preserve the true ranks of nans AND sort them to the beginning you can use:

*)

Rank.RankFirst(NanIsMaximum=false,RankNanWithNan=false) collection
// result:  [|4.0; 2.0; 5.0; 6.0; 1.0; 3.0|]



