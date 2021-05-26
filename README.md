# GurobiEvaluation
Solving group assignment problem with Gurobi

To run the demo, please install Gurobi Optimizer for dotNet (https://www.gurobi.com/products/gurobi-optimizer/)

The input for running is a folder (default name is test). Put the folder to the location of the run exe file. The program will scan the folder to create input data.
Note: The test folder has the structure like:
<br/>
+--- test<br/>
|   +--- Week1<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Introduction (T2 2892020)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Introduction (T4 3092020 - Tiết 012)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Introduction (T4 3092020 - Tiết 789)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Introduction (T5 01102020)-điểm.csv<br/>
|   +--- Week10<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Sequence Control (T2 07122020)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Sequence Control (T4 09122020 - Tiết 012)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Sequence Control (T4 09122020 - Tiết 789)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Sequence Control (T5 10122020)-điểm.csv<br/>
|   +--- Week11<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Control Abstraction (T2 14122020)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Control Abstraction (T4 16122020 - Tiết 012) (học bù)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Control Abstraction (T4 16122020 - Tiết 789)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Control Abstraction (T5 17122020) (học bù)-điểm.csv<br/>
|   +--- Week2<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Lexical Analysis (T2 2892020)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Lexical Analysis (T4 3092020 - Tiết 012)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Lexical Analysis (T4 3092020 - Tiết 789)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Lexical Analysis (T5 01102020)-điểm.csv<br/>
|   +--- Week4<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz OOP (T2 12102020)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz OOP (T4 14102020 - Tiết 012)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz OOP (T4 14102020 - Tiết 789)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz OOP (T5 234)-điểm.csv<br/>
|   +--- Week5<br/>
|   |   +--- .DS_Store<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Functional Programming (T2)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Functional Programming (T4 012)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Functional Programming (T4 789)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Functional Programming (T5)-điểm.csv<br/>
|   +--- Week7<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Name, Scope and Referencing Environment (T2 16112020)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Name, Scope and Referencing Environment (T4 11112020 - Tiết 012)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Name, Scope and Referencing Environment (T4 11112020 - Tiết 789)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz T7 (học bù)-điểm.csv<br/>
|   +--- Week8<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Type (T4 - Tiết 012)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Type (T4 - Tiết 789)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Type (T5 - Tiết 234)-điểm.csv<br/>
|   |   +--- CO3005_003904_DH_HK201-Quiz Type (T7 - 21112020)-điểm.csv
