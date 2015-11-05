#!/bin/sh
# Copy from upstream NUnit source code and then apply SharpDevelop's patch
export upstream=$1
cp $upstream/EventCollector.cs EventCollector.cs
cp $upstream/ConsoleUi.cs ExtendedConsoleUi.cs
cp $upstream/Runner.cs ExtendedRunner.cs
patch -i nunit.diff --no-backup-if-mismatch
