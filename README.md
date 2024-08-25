# Sistemsko programiranje

#  Contributors (Student, Br.Indeksa)
  - Anđela Stojanović, 18406
  - Anastasija Trajković, 18456

# 1.Projekat
  - Kreirati Web server koji klijentu omogućava pretagu umetničkih dela korišćenjem Metropolitan Museum of Art Collection API-a.
  - Pretraga se može vršiti pomoću filtera koji se definišu u okviru query-a.
  - Spisak umetničkih dela koje zadovoljavaju uslov se vraćaju kao odgovor klijentu.
  - Svi zahtevi serveru se šalju preko browser-a korišćenjem GET metode. Ukoliko navedena umetnička dela ne postoje, prikazati grešku klijentu.
  - Sinhrone operacije.

# 2.Projekat
  - Isti zadatak kao za 1. Projekat.
  - Ali koristiti taskove i asinhrone operacije (tamo gde to ima smisla).
  - Za obradu kod koje taskovi nemaju smisla treba zadržati klasične niti.
  - Dozvoljeno je korišćenje mehanizama za međusobno zaključavanje i sinhronizaciju.

# 3.Projekat
  - Koristeći principe Reaktivnog programiranja i News API, implementirati aplikaciju za prikaz naslova i izvora za određene članke (title i source property).
  - Koristiti /v2/top-headlines endpoint.
  - Prilikom poziva proslediti odgovarajuću ključnu reč (keyword), kao i kategoriju (category).
  - Za prikupljene naslove implementirati Topic Modeling koristeći SharpEntropy biblioteku. Prikazati dobijene rezultate.
  - Koristiti biblioteku Reactive Extensions for .NET (Rx) i implementirati odgovarajuće paradigme Reaktivnog programiranja.
  - Koristiti Postman alat za testiranje zahteva.
