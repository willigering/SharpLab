import json, sys, importlib
sys.path.insert(0, '.')
import gen
for m in ['teil1','teil2','teil3','teil4','teil5','begriffe','review','aufgabentexte','startcode','neu_l01_l09','neu_l10_l18','neu_l19_l27','neu_texte_1','neu_texte_2','neu_texte_3','neu_texte_4','neu_texte_5','neu_texte_6']:
    try: importlib.import_module(m)
    except ModuleNotFoundError as e:
        if e.name != m: raise
        print("fehlt:", m)
import neu_texte
neu_texte.anwenden()
ids=[l['id'] for l in gen.LESSONS]; assert len(ids)==len(set(ids))
out={"stages":gen.STAGES,"lessons":gen.LESSONS,"glossary":sorted(gen.GLOSSARY,key=lambda g:g['term'].lower())}
json.dump(out, open(__import__('os').path.join(__import__('os').path.dirname(__import__('os').path.abspath(__file__)),'..','..','src','CSharpLernen.Engine','Inhalte','Lektionen.json'),'w',encoding='utf-8'), ensure_ascii=False, indent=1)
print(len(gen.LESSONS),"Lektionen,", sum(len(l['exercises']) for l in gen.LESSONS),"Aufgaben,", sum(len(l['quiz']) for l in gen.LESSONS),"Quizfragen,", len(gen.GLOSSARY),"Begriffe")
