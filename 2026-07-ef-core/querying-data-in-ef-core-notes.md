
Pomenute funkcionalnosti:

- simple fetch
- `SelectMany()`
- `GroupBy`
- `GroupJoin`
- asynchronus queries - `ToListAsync`, `FirstAsync`, `AsAsyncEnumerable`...
- temporal tables
- stored procedures, views, raw SQL
    - storna procedura - u migraciju ručno dodati kod za `CREATE` i `DROP` procedure; paziti na SQL injection napade
    - view - kreiranje isto kao storna procedura; postavlja se kao entity + `.ToView()` metoda
    - `.FromSqlRaw()` ili `FromSqlInterpolated()`; paziti na SQL injection napade
- optimizing performance
    - `.AsNoTracking()`
    - paginacija (`.Skip(n)`, `.Take(m)`)
    - eager, lazy, explicit loading
    - projekcija kolona
