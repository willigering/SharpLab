import json, sys, importlib
sys.path.insert(0, '.')
import gen
for m in ['teil1','teil2','teil3','teil4','teil5','begriffe']:
    try: importlib.import_module(m)
    except ModuleNotFoundError as e:
        if e.name != m: raise
        print("fehlt:", m)
ids=[l['id'] for l in gen.LESSONS]; assert len(ids)==len(set(ids))
out={"stages":gen.STAGES,"lessons":gen.LESSONS,"glossary":sorted(gen.GLOSSARY,key=lambda g:g['term'].lower())}
json.dump(out, open('/workspace/CSharpLernen/src/CSharpLernen.Engine/Inhalte/Lektionen.json','w',encoding='utf-8'), ensure_ascii=False, indent=1)
print(len(gen.LESSONS),"Lektionen,", sum(len(l['exercises']) for l in gen.LESSONS),"Aufgaben,", sum(len(l['quiz']) for l in gen.LESSONS),"Quizfragen,", len(gen.GLOSSARY),"Begriffe")
