# BKWitten School App

## Projektübersicht

Dieses Projekt beinhaltet die Entwicklung einer Smartphone-App für die Berufsschule BKWitten. Die App dient als zentraler Kommunikationskanal, um Schüler mit wichtigen Informationen wie News, Veranstaltungen und Stundenplänen zu versorgen. Sie ergänzt oder ersetzt die Nutzung bestehender Plattformen wie Instagram und der Schulwebseite.

## Zielsetzung

- Zentraler, leicht zugänglicher und attraktiver Informationskanal für die Schüler der Berufsschule
- Ergänzung der bestehenden Plattformen durch eine benutzerfreundliche, plattformübergreifende App (iOS und Android)
- Erhöhung der Interaktion und Informationsverbreitung an die Schülerschaft

## Hauptfunktionen

### Für Schüler
- **News anzeigen**: Geordnete und gefilterte News
- **Push-Benachrichtigungen**: Für wichtige Nachrichten und Updates
- **Darkmode**: Umschaltung der App auf dunkles Design
- **Mehrsprachigkeit**: Unterstützung mehrerer Sprachen
- **Suche und Filter**: Möglichkeit, News und Informationen nach Kategorien zu filtern
- **Ereignisübersicht**: Anstehende Schulveranstaltungen und wichtige Termine
- **Gebäudeplan**: Orientierungshilfe durch Schulgebäudepläne
- **Lehrerinformationen**: Kontaktinformationen und Rollen der Lehrkräfte

### Für Lehrkräfte (Content Creator)
- **Beiträge erstellen**: Schnelles und einfaches Einpflegen von News
- **Medieninhalte hinzufügen**: Bilder, Videos oder Dokumente in Beiträge integrieren
- **Beiträge bearbeiten und löschen**: Nachträgliches Ändern oder Entfernen von Beiträgen
- **Geplante Veröffentlichung**: Zeitgesteuertes Veröffentlichen von Inhalten
- **Kategorien zuweisen**: Beiträge nach Fächern oder Veranstaltungen sortieren
- **Berechtigungen verwalten**: Zugriffskontrollen für verschiedene Schülergruppen

## Technologie-Stack

- **Programmiersprache**: C#
- **Frameworks**: .NET MAUI für die App, .NET ASP Core für das Backend
- **Datenbank**: SQLite für die Speicherung von Inhalten

## Installation

### Voraussetzungen

- .NET SDK (mindestens Version 6.0)
- Visual Studio Code oder Visual Studio 2022
- SQLite

### Schritte zur Installation

1. Repository klonen:
   
    git clone https://github.com/DeinBenutzername/BKWittenSchoolApp.git

    In das Projektverzeichnis wechseln:


cd BKWittenSchoolApp

Projektabhängigkeiten installieren:

bash

dotnet restore

Datenbankmigrationen anwenden (falls erforderlich):

bash

dotnet ef database update

Anwendung starten:

bash

    dotnet run

Verwendung

    Öffne die App auf einem iOS- oder Android-Gerät.

Tests

Um die Anwendung zu testen, führen Sie den folgenden Befehl im Hauptverzeichnis aus:

bash

dotnet test

Risiken und Herausforderungen

    Akzeptanz durch die Schüler könnte gering sein, daher sind regelmäßige Benutzerbefragungen geplant.
    Die genutzten Frameworks sind teilweise neu für das Team, was zusätzliche Einarbeitung erfordert.

Abnahmekriterien

    Die App ist auf iOS und Android lauffähig und erfüllt alle im Pflichtenheft definierten Anforderungen.
    Positive Bewertung durch Benutzertests.
    Vollständige Dokumentation und Wartungshandbuch.

Dokumentation

    Benutzerhandbuch für Lehrkräfte: Enthält Anweisungen zum Erstellen und Verwalten von Beiträgen.
    Wartungsdokumentation: Informationen zur langfristigen Pflege und Erweiterung der App.

