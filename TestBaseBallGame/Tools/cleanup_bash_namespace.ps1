param()

$root = "Assets\00.Scripts"
$files = Get-ChildItem -Path $root -Recurse -Filter '*.cs' -ErrorAction SilentlyContinue
if (-not $files) { Write-Output "No files found under $root"; exit }

function Remove-NamespaceBlocks([string]$text) {
 $pattern = 'namespace\s+Bash[^\{]*\{'
 $modified = $false
 while ($text -match $pattern) {
 $m = [regex]::Match($text, $pattern)
 $startDecl = $m.Index
 $openIndex = $text.IndexOf('{', $startDecl)
 if ($openIndex -lt0) { break }
 # find matching closing brace
 $depth =1
 $i = $openIndex +1
 while ($i -lt $text.Length -and $depth -gt0) {
 if ($text[$i] -eq '{') { $depth++ }
 elseif ($text[$i] -eq '}') { $depth-- }
 $i++
 }
 if ($depth -ne0) { break }
 $closeIndex = $i -1
 $before = $text.Substring(0, $startDecl)
 $inner = $text.Substring($openIndex +1, $closeIndex - $openIndex -1)
 $after = $text.Substring($closeIndex +1)
 $text = $before + $inner + $after
 $modified = $true
 }
 return @{ Text = $text; Modified = $modified }
}

foreach ($f in $files) {
 try {
 $text = Get-Content -Raw -Encoding UTF8 -Path $f.FullName
 } catch {
 $text = Get-Content -Raw -Path $f.FullName
 }
 $orig = $text
 # remove using Bash.* lines
 $text = [regex]::Replace($text, "(?m)^\s*using\s+Bash\.[^\r\n]*\r?\n", "")

 $res = Remove-NamespaceBlocks $text
 $text = $res.Text

 if ($text -ne $orig) {
 try { Set-Content -Path $f.FullName -Value $text -Encoding UTF8; Write-Output "Modified: $($f.FullName)" } catch { Write-Output "Failed write: $($f.FullName) - $_" }
 }
}

Write-Output "Done."