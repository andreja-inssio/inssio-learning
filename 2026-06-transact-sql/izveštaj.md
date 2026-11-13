# Izveštaj

## Sažetak

Kurs https://learn.microsoft.com/en-us/training/paths/write-advanced-transact-sql-queries/ mi je bio koristan. Svaka celina je jasno objašnjena sa pojmovima specifičnim za T-SQL. Sa nekima sam se susretao u prošlosti, dok sa nekima nisam.

Svaka celina je objašnjena tekstualno i kroz kôd. Objašnjenja su jasna, koncizna i praćena primerima. Na kraju svakog modula su date pokazne vežbe sa malo izazovnijim primerima. Vežbe su opcione za sertifikat o polaganju kursa, ali preporučujem da se urade ručno, bez korišćenja LLM alata.

## Primena

Potencijalna primena nekih celina pokrivenih unutar kursa na CTABS projektu:

- **views** - projekat već koristi poglede. Poznavanje pogleda je neophodno za rad na projektu.
- **temporary tables** - projekat već koristi privremene tabele (npr. `USP_CTABSE2_SEARCH_CLAIMS` procedura). U zavisnosti od potrebe, mogu se učitati lokalne privremene tabele unutar stornih procedura koje će se koristiti više puta unutar iste procedure.
- **common table expressions** - korisno za kompleksne upite za grid paginaciju. Prvo se definiše CTE unutar storne procedure koji sadrži sve neophodne operacije za upit (`SELECT`, `WHERE`...). CTE potom primenjuje `COUNT(*)` i `SKIP/TAKE` operacije kako bismo iz jednog puta ka bazi podataka izvukli ukupan broj slogova i sve neophodne slogove za taj upit.
- **derived tables** - slično kao za podupite. Koristi se radi lakšeg pisanja složenih upita.
- **UNION, INTERSECT, EXCEPT** - koristi se za skupovne operacije gde je neophodno.
- **APPLY** - koristiti u slučajevima kada je `JOIN` operacija skupa ili nepraktična. Npr. ako želimo da uradimo spajanje tabele sa `TOP N` slogova neke druge tabele, `APPLY` operacija je praktičnija.

```sql
SELECT  t1.*, t2o.*
FROM    t1
CROSS APPLY
(
    -- teže je formulisati sa JOIN operacijom
    SELECT  TOP 3 *
    FROM    t2
    WHERE   t2.t1_id = t1.id
    ORDER BY
            t2.rank DESC
) t2o
```

- **windows functions (OVER clause)** - koriste se na CTABS-u. Uobičajena primena sadrži `OVER`, `PARTITION BY`, `ORDER BY`. Primenjuje se `ROW_NUMBER()` za dodeljivanje broja slogu radi rangiranja. Pojedini primeri koriste i funkcije agregacije (`MIN` za prioritet i `SUM` za zbir rezervacija).
- **PIVOT/UNPIVOT** - ne koriste se. Potencijalna primena je generisanje izveštaja po godinama za razne kriterijume.
- **GROUPING SETS/CUBE/ROLLUP** - ne koriste se.