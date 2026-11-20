<?php
// SharpLab – Feedback-Empfänger (Vorlage, NICHT hochgeladen). Erwartet POST application/json:
// {subject, message, email, appVersion, os, website (Honeypot, leer), log (optional)} und Header X-SharpLab-Client: 1.
// Antwort: {"ok":true} bzw. {"ok":false,"error":"..."}.
declare(strict_types=1);
header('Content-Type: application/json; charset=utf-8');
const EMPFAENGER = 'info@geringdigital.de';
const ABSENDER   = 'sharplab@geringdigital.de';      // muss eine Adresse der eigenen Domain sein (SPF/DMARC)
const LIMIT_PRO_STUNDE = 10;

function antwort(int $code, bool $ok, string $fehler = ''): void {
    http_response_code($code);
    echo json_encode($ok ? ['ok' => true] : ['ok' => false, 'error' => $fehler], JSON_UNESCAPED_UNICODE);
    exit;
}
function zeile(string $s, int $max): string {               // eine Zeile, keine Header-Injection
    $s = trim(preg_replace('/[\r\n\t\x00-\x1F\x7F]+/u', ' ', $s) ?? '');
    return mb_substr($s, 0, $max);
}

if ($_SERVER['REQUEST_METHOD'] !== 'POST') antwort(405, false, 'Nur POST.');
if (($_SERVER['HTTP_X_SHARPLAB_CLIENT'] ?? '') !== '1') antwort(403, false, 'Unbekannter Client.');
$roh = file_get_contents('php://input', false, null, 0, 64 * 1024);
$d = json_decode($roh ?: '', true);
if (!is_array($d)) antwort(400, false, 'Ungültige Daten.');
if (($d['website'] ?? '') !== '') antwort(200, true);       // Honeypot gefüllt → still verwerfen

$betreff = zeile((string)($d['subject'] ?? ''), 120);
$text    = trim((string)($d['message'] ?? ''));
$email   = trim((string)($d['email'] ?? ''));
$version = zeile((string)($d['appVersion'] ?? '?'), 20);
$os      = zeile((string)($d['os'] ?? '?'), 120);
$log     = mb_substr((string)($d['log'] ?? ''), -20480);
if ($text === '' || mb_strlen($text) > 5000) antwort(400, false, 'Nachricht fehlt oder ist zu lang.');
if ($email !== '' && !filter_var($email, FILTER_VALIDATE_EMAIL)) $email = '';

// einfaches Limit pro IP (Datei im Temp-Ordner)
$ipHash = hash('sha256', ($_SERVER['REMOTE_ADDR'] ?? '') . __FILE__);
$datei = sys_get_temp_dir() . "/sharplab-fb-$ipHash.json";
$jetzt = time();
$zeiten = array_values(array_filter(json_decode(@file_get_contents($datei) ?: '[]', true) ?: [], fn($t) => $t > $jetzt - 3600));
if (count($zeiten) >= LIMIT_PRO_STUNDE) antwort(429, false, 'Zu viele Nachrichten – bitte später nochmal.');
$zeiten[] = $jetzt;
@file_put_contents($datei, json_encode($zeiten), LOCK_EX);

$subject = 'SharpLab: ' . ($betreff !== '' ? $betreff : 'Feedback');
$body = $text . "\n\n---\nSharpLab $version · $os\n" . ($email !== '' ? "Antwort an: $email\n" : "Keine Antwortadresse angegeben.\n");
if ($log !== '') $body .= "\n--- fehler.log (Auszug) ---\n" . $log . "\n";
$header = ['From' => 'SharpLab Feedback <' . ABSENDER . '>', 'Content-Type' => 'text/plain; charset=UTF-8', 'X-SharpLab-Version' => $version];
if ($email !== '') $header['Reply-To'] = $email;

$ok = mail(EMPFAENGER, '=?UTF-8?B?' . base64_encode($subject) . '?=', $body, $header);
$ok ? antwort(200, true) : antwort(500, false, 'Die Nachricht konnte nicht zugestellt werden.');
